using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Utility;

namespace SharpNeat.Domains.Classification.dota2
{

    /// <summary>
    /// A concrete implementation of IGenomeListEvaluator that evaulates genomes independently of each 
    /// other and in parallel (on multiple execution threads).
    /// 
    /// Genome decoding is performed by a provided IGenomeDecoder.
    /// Phenome evaluation is performed by a provided IPhenomeEvaluator.
    /// </summary>
    public class Dota2GenomeListEvaluator<TGenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        readonly IGenomeDecoder<TGenome, IBlackBox> _genomeDecoder;
        readonly Dota2BlackBoxEvaluator _phenomeEvaluator;
        readonly ParallelOptions _parallelOptions;
        readonly bool _enablePhenomeCaching;
        private readonly Dota2DataProvider _dataProvider;
        readonly EvaluationMethod _evalMethod;
        FastRandom rand = new FastRandom();
        private static bool flag = true;

        delegate void EvaluationMethod(IList<TGenome> genomeList);

        #region Constructors

        /// <summary>
        /// Construct with the provided IGenomeDecoder, IPhenomeEvaluator, ParalleOptions and enablePhenomeCaching flag.
        /// </summary>
        public Dota2GenomeListEvaluator(IGenomeDecoder<TGenome, IBlackBox> genomeDecoder,
                                           Dota2BlackBoxEvaluator phenomeEvaluator,
                                           ParallelOptions options,
                                           bool enablePhenomeCaching,
                                           Dota2DataProvider dataProvider)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = options;
            _enablePhenomeCaching = enablePhenomeCaching;
            _dataProvider = dataProvider;

            // Determine the appropriate evaluation method.
            if (_enablePhenomeCaching)
            {
                _evalMethod = Evaluate_Caching;
            }
            else
            {
                _evalMethod = Evaluate_NonCaching;
            }
            File.WriteAllText("evaluator_seed.txt", "" + rand.seed);
        }

        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }

        /// <summary>
        /// Evaluates a list of genomes. Here we decode each genome in using the contained IGenomeDecoder
        /// and evaluate the resulting TPhenome using the contained IPhenomeEvaluator.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList)
        {
            _evalMethod(genomeList);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode on each loop).
        /// </summary>
        private void Evaluate_NonCaching(IList<TGenome> genomeList)
        {
            //var indexes = getIndexes(_dataProvider.getData(), 0.3);
            Parallel.ForEach(genomeList, _parallelOptions, delegate (TGenome genome)
            {
                IBlackBox phenome = _genomeDecoder.Decode(genome);
                if (null == phenome)
                {   // Non-viable genome.
                    genome.EvaluationInfo.SetFitness(0.0);
                    genome.EvaluationInfo.AuxFitnessArr = null;
                }
                else
                {
                    //_phenomeEvaluator.Indexes = indexes;
                    FitnessInfo fitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                    genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                    genome.EvaluationInfo.SetEvalFitness(fitnessInfo._evalFitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                }
            });
        }


        /// <summary>
        /// Main genome evaluation loop with phenome caching (decode only if no cached phenome is present
        /// from a previous decode).
        /// </summary>
        private void Evaluate_Caching(IList<TGenome> genomeList)
        {
            List<int> indexes;
            if (flag)
            {
                //indexes = getIndexes(_dataProvider.getData(), 1);
                Parallel.ForEach(genomeList, _parallelOptions, delegate (TGenome genome)
                {
                    IBlackBox phenome = (IBlackBox)genome.CachedPhenome;
                    if (null == phenome)
                    {
                    // Decode the phenome and store a ref against the genome.
                    phenome = _genomeDecoder.Decode(genome);
                        genome.CachedPhenome = phenome;
                    }

                    if (null == phenome)
                    {
                    // Non-viable genome.
                    genome.EvaluationInfo.SetFitness(0.0);
                        genome.EvaluationInfo.AuxFitnessArr = null;
                    }
                    else
                    {
                        _phenomeEvaluator.Indexes = null;
                        FitnessInfo fitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                        genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                    //genome.EvaluationInfo.SetEvalFitness(fitnessInfo._evalFitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                    }
                });
                flag = false;
                return;
            }

            indexes = getIndexes(_dataProvider.getData(), 0.2);
            Parallel.ForEach(genomeList, _parallelOptions, delegate (TGenome genome)
            {
                IBlackBox phenome = (IBlackBox)genome.CachedPhenome;
                if (null == phenome)
                {   // Decode the phenome and store a ref against the genome.
                    phenome = _genomeDecoder.Decode(genome);
                    genome.CachedPhenome = phenome;
                }

                if (null == phenome)
                {   // Non-viable genome.
                    genome.EvaluationInfo.SetFitness(0.0);
                    genome.EvaluationInfo.AuxFitnessArr = null;
                }
                else
                {
                    _phenomeEvaluator.Indexes = indexes;
                    FitnessInfo fitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                    genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                    genome.EvaluationInfo.SetEvalFitness(fitnessInfo._evalFitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                }
            });
            flag = true;
        }

        private List<int> getIndexes(Dataset dataset, double subSample)
        {
            int samplesCount;
            if (subSample > 0)
                samplesCount = (int)(subSample * dataset.Samples.Count);
            else
                samplesCount = dataset.Samples.Count;
            List<int> result;
            if (samplesCount == dataset.Samples.Count)
            {
                result = new List<int>(samplesCount);
                for (int i = 0; i < samplesCount; i++)
                {
                    result.Add(i);
                }
                return result;
            }
            result = new List<int>(samplesCount);
            HashSet<int> check = new HashSet<int>();
            for (int i = 0; i < samplesCount; i++)
            {
                int curValue = rand.Next(0, dataset.Samples.Count);
                while (check.Contains(curValue))
                {
                    curValue = rand.Next(0, dataset.Samples.Count);
                }
                result.Add(curValue);
                check.Add(curValue);
            }
            return result;
        }

        #endregion
    }

}
