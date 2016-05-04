using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Domains.Classification
{
    public class BinaryEvaluator
    {
        private readonly List<int> _indexes;
        int correctlyClassified;
        int incorrectlyClassified;

        public BinaryEvaluator() { }

        public BinaryEvaluator(List<int> indexes )
        {
            _indexes = indexes;
        }

        public EvaluateInfo EvaluateTestData(IBlackBox box, Dataset dataset)
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
                activateTest(box, inputs, outputs);
                predictedProbs[i] = outputs[0];
                expectedProbs[i] = expected[0];
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int)x == (int)y ? 1.0 : 0.0);
            info.accuracy = temp.Average();
            info.auc = Auc(expectedProbs, predictedProbs);
            //info.auc = Auc2(predictedProbs, expectedProbs);
            return info;
        }
        public EvaluateInfo Evaluate(IBlackBox box, Dataset dataset)
        {
            if (_indexes != null) return Evaluate2(box, dataset);
            EvaluateInfo info = new EvaluateInfo();
            double[] predictedProbs = new double[dataset.Samples.Count];
            double[] expectedProbs = new double[dataset.Samples.Count];
            for (int i = 0; i < dataset.Samples.Count; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs

                activate(box, inputs, outputs);
                predictedProbs[i] = outputs[0];
                expectedProbs[i] = expected[0];
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int)x == (int)y ? 1.0 : 0.0);
            info.accuracy = temp.Average();
            info.auc = Auc(expectedProbs, predictedProbs);
            //info.auc = Auc2(predictedProbs, expectedProbs);
            return info;
        }
        public EvaluateInfo Evaluate2(IBlackBox box, Dataset dataset)
        {
            EvaluateInfo info = new EvaluateInfo();
            double[] predictedProbs = new double[_indexes.Count];
            double[] expectedProbs = new double[_indexes.Count];
            for (int i = 0; i < _indexes.Count; i++)
            {
                var index = _indexes[i];
                DataRow dataRow = dataset.Samples[index];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs

                activate(box, inputs, outputs);
                predictedProbs[i] = outputs[0];
                expectedProbs[i] = expected[0];
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int) x == (int) y ? 1.0 : 0.0);
            info.accuracy = temp.Average();
            info.auc = Auc(expectedProbs, predictedProbs);
            //info.auc = Auc2(predictedProbs, expectedProbs);
            return info;
        }

        private double[] activateTest(IBlackBox box, IList<double> inputs, double[] outputs)
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
            box.ActivateWithDropout();
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
            long truePos = tp0 = ones;
            long accum = tn = 0;
            double threshold = all[0].predictedValue;
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

        private void calculateCorrectness(List<double> expected, double[] outputs)
        {
            int correct = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                double expect = expected[i];
                double output = outputs[i];
                if (binarize(expect) == binarize(output))
                {
                    correct++;
                }
            }
            if (correct == outputs.Length)
            {
                correctlyClassified++;
            }
            else
            {
                incorrectlyClassified++;
            }
        }

        private double[] binarize(double[] values)
        {
            double[] result = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = binarize(values[i]);
            }
            return result;
        }

        private int binarize(double value)
        {
            return value >= 0.5 ? 1 : 0;
        }
    }
}
