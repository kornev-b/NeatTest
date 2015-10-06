using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Domains.Mine.Iris
{
    class IrisBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        const int AcceptedAccuracy = 95; // in percent
        IrisDataProvider dataProvider = new IrisDataProvider();

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
            int samplesCount = dataset.Samples.Count;
            var results = new ResultType[samplesCount][];
            for (int i = 0; i < samplesCount; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs
                activate(box, inputs, outputs);
                // apply to each pair of outputs array element and expected array element
                // a function that is determined what kind of ResultType
                // this pair produces (TP, TN, FN, TN)
                results[i] = outputs.Zip(expected, (o, e) => getResultType(o, e)).ToArray();
            }

            // Compute per-column sums
            // to know how much TPs, TNs, FPs, FNs
            // we have for each dataset output 
            var TPs = new int[dataset.OutputCount];
            var TNs = new int[dataset.OutputCount];
            var FPs = new int[dataset.OutputCount];
            var FNs = new int[dataset.OutputCount];
            for (var i = 0; i < dataset.OutputCount; i++)
            {
                for (var j = 0; j < samplesCount; j++)
                {
                    TPs[i] += (results[j][i] == ResultType.TP) ? 1 : 0;
                    TNs[i] += (results[j][i] == ResultType.TN) ? 1 : 0;
                    FPs[i] += (results[j][i] == ResultType.FP) ? 1 : 0;
                    FNs[i] += (results[j][i] == ResultType.FN) ? 1 : 0;
                }
            }

            /*
             -------------------------------------------------------------
            |                     | Expected positive | Expected negative |          
             -------------------------------------------------------------
            |  Predicted positive |         TP        |        FP         |
             -------------------------------------------------------------
            |  Predicted negative |         FN        |        TN         |
             -------------------------------------------------------------
            */

            // predicted positive and expected positive
            var TP = TPs.Average();
            // predicted negative and expected negative 
            var TN = TNs.Average();
            // predicted negative but expected positive  
            var FP = FPs.Average();
            // predicted positive but expected negative 
            var FN = FNs.Average();

            // ratio of the number of correctly predicted positive (relevant)
            // retrieved to the total number of incorrectly predicted (irrelevant)
            // positive values and relevant values 
            var precision = TP/(TP + FP);
            // ratio of the number of correctly predicted positive (relevant)
            // retrieved to the total number of incorrectly predicted (irrelevant)
            // negative values and relevant values
            var recall = TP/(TP + FN);
            var accuracy = (TP + TN)/(TP + TN + FP + FN);
            double FMeasure;
            if (precision + recall == 0)
            {
                FMeasure = 0d;
            }
            else
            {
             FMeasure = 2 * (precision * recall / (precision + recall));
            }
            double fitness = FMeasure;
            if (fitness < 0.0 || double.IsNaN(fitness) || double.IsInfinity(fitness))
            {
                fitness = 0;
            }
            if (accuracy >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }
            return new FitnessInfo(fitness, fitness);
        }

        private double[] activate(IBlackBox box, IList<double> inputs, double[] outputs)
        {
            box.ResetState();

            // Give inputs to the network
            var nodeId = 0;
            foreach (var input in inputs)
            {
                box.InputSignalArray[nodeId++] = input;
            }

            // Activate the network and get outputs back
            box.Activate();
            box.OutputSignalArray.CopyTo(outputs, 0);

//            normalizeOutputs(outputs);

            return outputs;
        }

        private void normalizeOutputs(double[] outputs)
        {
            for (var i = 0; i < outputs.Count(); i++)
            {
                outputs[i] = (outputs[i] + 1.0) / 2.0;
            }
        }

        /// <summary>
        /// Interpret a result as a true positive, false positive, true negative or false negative.
        /// </summary>
        private IrisBlackBoxEvaluator.ResultType getResultType(double output, double expected)
        {
            var binOutput = binarize(output);
            var binExpected = binarize(expected);
//            if (output < 0.5)
//            {
//                Debug.WriteLine("Output less than 0.5: " + output);
//            }
            bool isCorrect = binOutput == binExpected;
            if (binOutput == 1)
            {
                return isCorrect ? ResultType.TP : ResultType.FP;
            }
            else
            {
                return isCorrect ? ResultType.TN : ResultType.FN;
            }
        }

        private int binarize(double value)
        {
            return value >= 0.5 ? 1 : 0;
        }

        private double ratioOfCorrectOutputForValue(int value, IList<int> outputs, IList<int> expected)
        {
            int nbCorrectValue = 0;
            int nbEqualsValue = 0;
            for (int i = 0; i < outputs.Count(); i++)
            {
                nbCorrectValue += (outputs[i] == value && expected[i] == value ? 1 : 0);
                nbEqualsValue += (outputs[i] == value ? 1 : 0);
            }
            return nbEqualsValue > 0 ? (double)nbCorrectValue / nbEqualsValue : 0.0;
        }

        public enum ResultType
        {
            TP, TN, FP, FN
        }

        public void Reset()
        {
        }
    }
}
