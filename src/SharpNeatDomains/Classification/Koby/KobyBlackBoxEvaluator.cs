using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Domains.Classification.Koby
{
    public class KobyBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1d;
        Evaluator evaluator = new Evaluator();
        public List<int> Indexes { get; set; }
        public OverfittingParams _overfittingParams = new OverfittingParams();

        public KobyBlackBoxEvaluator()
        {
            Fitness = Fitness.ACCURACY;
        }

        public DataProvider DataProvider { get; set; }

        public Fitness Fitness { get; set; }

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            BinaryEvaluator binaryEvaluator = new BinaryEvaluator(Indexes, EvaluateInfo.Metric.LOGLOSS);
            binaryEvaluator._overfittingParams = _overfittingParams;
            _evalCount++;
            var dataset = DataProvider.getData();
            EvaluateInfo info = binaryEvaluator.Evaluate(phenome, dataset);
            if (_overfittingParams.l2enabled)
                regularize(info, phenome);
            if (info.logloss == 0)
            {
                _stopConditionSatisfied = true;
                return new FitnessInfo(info.logloss, info.logloss);
            }
            info.logloss = 1/info.logloss;
            //EvaluateInfo evaInfo = binaryEvaluator.EvaluateTestData(phenome, evaDataset);
            
            return new FitnessInfo(info.logloss, info.logloss);
        }

        private void regularize(EvaluateInfo info, IBlackBox phenome)
        {
            FastAcyclicNetwork fan = (FastAcyclicNetwork)phenome;
            double decay = 0;
            foreach (var con in fan.Connections)
            {
                decay += Math.Pow(con._weight, 2);
            }
            decay = decay * _overfittingParams.l2 / (2 * fan.Connections.Length);
            info.auc -= decay;
        }

        public void Reset()
        {
        }
    }
}
