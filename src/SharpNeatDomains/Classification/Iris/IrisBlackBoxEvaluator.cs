using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification.Iris
{
    public enum Fitness
    {
        FMEASURE,
        ACCURACY,
        PRECISION,
        RECALL
    }

    class IrisBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1.00d;
        Evaluator evaluator = new Evaluator();

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


        public FitnessInfo Evaluate(IBlackBox box)
        {
            _evalCount++;
            var dataset = DataProvider.getData();
            EvaluateInfo info = evaluator.Evaluate(box, dataset);
        
            if (info.Accuracy >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }
            double fitness;
            switch (Fitness)
            {
                case Fitness.ACCURACY:
                    fitness = info.Accuracy;
                    break;
                case Fitness.FMEASURE:
                    fitness = info.FMeasure;
                    break;
                case Fitness.PRECISION:
                    fitness = info.Precision;
                    break;
                case Fitness.RECALL:
                    fitness = info.Recall;
                    break;
                default:
                    fitness = info.FMeasure;
                    break;                
            }
             
            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
        }
    }
}
