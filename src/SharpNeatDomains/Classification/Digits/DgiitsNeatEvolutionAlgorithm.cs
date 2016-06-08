using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace SharpNeat.Domains.Classification.Digits
{
    public class DgiitsNeatEvolutionAlgorithm:NeatEvolutionAlgorithm<NeatGenome>
    {
        private double bestFitness;
        private const double FITNESS_THRESHOLD = 0.5;
        private readonly DigitsDataProvider _dataProvider;
        private List<int> generationTrack = new List<int>(new int[]{0, 50, 100, 500});
        XmlWriterSettings _xwSettings = new XmlWriterSettings();
 
        public DgiitsNeatEvolutionAlgorithm(DigitsDataProvider dataProvider, NeatEvolutionAlgorithmParameters eaParams, ISpeciationStrategy<NeatGenome> speciationStrategy, IComplexityRegulationStrategy complexityRegulationStrategy) : base(eaParams, speciationStrategy, complexityRegulationStrategy)
        {
            _dataProvider = dataProvider;
            _xwSettings.Indent = true;
        }

        public void Seed(int seed)
        {
            _rng.Reinitialise(seed);
        }

        protected override void UpdateBestGenome()
        {
            
            base.UpdateBestGenome();
            if (_currentBestGenome == null) return;
            BinaryEvaluator binaryEvaluator = new BinaryEvaluator();

            var eval = binaryEvaluator.EvaluateTestData((IBlackBox) _currentBestGenome.CachedPhenome, _dataProvider.getEvalData());
            _currentBestGenome.EvaluationInfo.SetEvalFitness(eval.auc);
            if (eval.auc > FITNESS_THRESHOLD && eval.auc > bestFitness)
            {
                bestFitness = eval.auc;
                saveChampion();
            }
            //uint generation = _currentBestGenome.BirthGeneration;
            //if (generationTrack.Count > 0 && generation >= generationTrack[0])
            //{
            //    generationTrack.RemoveAt(0);
            //    predictProba(_currentBestGenome);
            //    saveChampion();
            //} else if (generationTrack.Count == 0 && generation >= 1000)
            //{
            //    predictProba(_currentBestGenome);
            //    saveChampion();
            //}
        }

        private void saveChampion()
        {
            string spath = "champ.gnm.xml";
            
            // Save genome to xml file.
            using (XmlWriter xw = XmlWriter.Create(spath, _xwSettings))
            {
                NeatGenomeXmlIO.WriteComplete(xw, new NeatGenome[] { _currentBestGenome }, false);
                //NeatGenomeXmlIO.Write(xw, _currentBestGenome, false);
            }
        }

        private void predictProba(NeatGenome genome)
        {
            IBlackBox phenome = (IBlackBox)genome.CachedPhenome;
            var evaluator = new ProbabilitiesEvaluator();
            var info = evaluator.predictProba(phenome, _dataProvider.getData());

            // set a default file name
            var filename = "predicted_proba_train" + genome.BirthGeneration + ".txt";

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < info.Length; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < info[i].Length; j++)
                    {
                        sb.Append(string.Format("{0:0.00000000000000}", info[i][j])).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());
                }
            }

            info = evaluator.predictProba(phenome, _dataProvider.getEvalData());
            filename = "predicted_proba_" + genome.BirthGeneration + ".txt";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int i = 0; i < info.Length; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < info[i].Length; j++)
                    {
                        sb.Append(string.Format("{0:0.00000000000000}", info[i][j])).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());
                }
            }
        }
    }
}
