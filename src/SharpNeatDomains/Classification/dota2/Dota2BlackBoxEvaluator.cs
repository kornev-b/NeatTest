using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification.dota2
{
    public class Dota2BlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const double AcceptedAccuracy = 1.00d;
        BinaryEvaluator evaluator = new BinaryEvaluator();

        public Dota2BlackBoxEvaluator()
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
            EvaluateInfo info = evaluator.evaluate(phenome, dataset);

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
