using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification.Iris
{
    class IrisBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1.00d;
        IrisDataProvider dataProvider = new IrisDataProvider();
        Evaluator evaluator = new Evaluator();

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }


        public FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;
            var dataset = dataProvider.getData();
            EvaluateInfo info = evaluator.Evaluate(box, dataset);
        
            if (info.Accuracy >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }
            double fitness = info.FMeasure;
            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
        }
    }
}
