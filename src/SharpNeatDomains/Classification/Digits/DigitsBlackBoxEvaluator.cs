using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Domains.Classification.Digits
{
    public class DigitsBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1d;
        Evaluator evaluator = new Evaluator();
        public List<int> Indexes { get; set; }
        public OverfittingParams _overfittingParams = new OverfittingParams();

        public DigitsBlackBoxEvaluator()
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
            _evalCount++;
            var dataset = DataProvider.getData();
            EvaluateInfo info = evaluator.Evaluate(phenome, dataset);
            if (_overfittingParams.l2enabled)
                regularize(info, phenome);
            FastAcyclicNetwork fan = (FastAcyclicNetwork)phenome;
            if (info.Accuracy >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }
            
            return new FitnessInfo(info.Accuracy, info.Accuracy);
        }

        private void regularize(EvaluateInfo info, IBlackBox phenome)
        {
            FastAcyclicNetwork fan = (FastAcyclicNetwork) phenome;
            double decay = 0;
            foreach (var con in fan.Connections)
            {
                decay += Math.Pow(con._weight, 2);
            }
            decay = decay*_overfittingParams.l2/(2*fan.Connections.Length);
            info.auc -= decay;
        }

        public void Reset()
        {
        }
    }
}
