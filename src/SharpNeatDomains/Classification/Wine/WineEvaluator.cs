using SharpNeat.Core;
using System;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification.Wine
{
    class WineEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;

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
            var evaluator = new Evaluator();
            var provider = new WineDataProvider();
            var dataset = provider.getData();
            var info = evaluator.Evaluate(phenome, dataset);
            double fitness = info.FMeasure;
            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
            _evalCount = 0;
            _stopConditionSatisfied = false;
        }
    }
}
