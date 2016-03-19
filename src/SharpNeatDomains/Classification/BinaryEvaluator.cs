using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification
{
    public class BinaryEvaluator
    {
        public EvaluateInfo evaluate(IBlackBox box, Dataset dataset)
        {
            EvaluateInfo info = new EvaluateInfo();
            int samplesCount = dataset.Samples.Count;
            double[] predictedProbs = new double[samplesCount];
            double[] expectedProbs = new double[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs
                activate(box, inputs, outputs);
                predictedProbs[i] = outputs[1];
                expectedProbs[i] = expected[1];
            }
            info.auc = Auc(expectedProbs, predictedProbs);
            return info;
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

        public static double Auc(double[] a, double[] p)
        {
            // AUC requires int array as dependent

            var all = a.Zip(p,
                            (actual, pred) => new { actualValue = actual < 0.5 ? 0 : 1, predictedValue = pred })
                       .OrderBy(ap => ap.predictedValue)
                       .ToArray();

            long n = all.Length;
            long ones = all.Sum(v => v.actualValue);
            if (0 == ones || n == ones) return 1;

            long tp0, tn;
            long truePos = tp0 = ones; long accum = tn = 0; double threshold = all[0].predictedValue;
            for (int i = 0; i < n; i++)
            {
                if (all[i].predictedValue != threshold)
                { // threshold changes
                    threshold = all[i].predictedValue;
                    accum += tn * (truePos + tp0); //2* the area of  trapezoid
                    tp0 = truePos;
                    tn = 0;
                }
                tn += 1 - all[i].actualValue; // x-distance between adjacent points
                truePos -= all[i].actualValue;
            }
            accum += tn * (truePos + tp0); // 2 * the area of trapezoid
            return (double)accum / (2 * ones * (n - ones));
        }
    }
}
