using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification.Taxi
{
    public class TaxiBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1d;
        Evaluator evaluator = new Evaluator();
        public List<int> Indexes { get; set; }
        public OverfittingParams OverfittingParams;

        public TaxiBlackBoxEvaluator()
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
            BinaryEvaluator binaryEvaluator = new BinaryEvaluator(Indexes);
            binaryEvaluator._overfittingParams = OverfittingParams;
            _evalCount++;
            var dataset = DataProvider.getData();
            EvaluateInfo info = binaryEvaluator.Evaluate(phenome, dataset);
            //EvaluateInfo evaInfo = binaryEvaluator.EvaluateTestData(phenome, evaDataset);
            if (info.auc >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }
            
            return new FitnessInfo(info.auc, info.auc);
        }

        public void Reset()
        {
        }
    }
}
