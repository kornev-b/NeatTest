using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private EvaluateInfo.Metric _metric = EvaluateInfo.Metric.AUC;
        int correctlyClassified;
        int incorrectlyClassified;

        public OverfittingParams _overfittingParams;

        public BinaryEvaluator() { }

        public BinaryEvaluator(List<int> indexes )
        {
            _indexes = indexes;
        }

        public BinaryEvaluator(List<int> indexes, EvaluateInfo.Metric metric)
        {
            _indexes = indexes;
            _metric = metric;
        }

        public EvaluateInfo EvaluateTestData(IBlackBox box, Dataset dataset)
        {
            EvaluateInfo info = new EvaluateInfo(_metric);
            int samplesCount = dataset.Samples.Count;
            List<double> predictedProbs = new List<double>(dataset.Samples.Count);
            List<double> expectedProbs = new List<double>(dataset.Samples.Count);

            for (int i = 0; i < samplesCount; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs
                activateTest(box, inputs, outputs);
                predictedProbs.Add(outputs[0]);
                expectedProbs.Add(expected[0]);
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int)x == (int)y ? 1.0 : 0.0);
            info.accuracy = temp.Average();
            info.auc = Auc(expectedProbs, predictedProbs);
            info.logloss = logloss(expectedProbs, predictedProbs);
            //info.auc = Auc2(predictedProbs, expectedProbs);
            return info;
        }

        public EvaluateInfo Evaluate(IBlackBox box, Dataset dataset)
        {
            if (_indexes != null) return Evaluate2(box, dataset);
            EvaluateInfo info = new EvaluateInfo(_metric);
            List<double> predictedProbs = new List<double>(dataset.Samples.Count);
            List<double> expectedProbs = new List<double>(dataset.Samples.Count);
            for (int i = 0; i < dataset.Samples.Count; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs

                activate(box, inputs, outputs);
                predictedProbs.Add(outputs[0]);
                expectedProbs.Add(expected[0]);
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            //var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int)x == (int)y ? 1.0 : 0.0);
            //info.accuracy = temp.Average();
            switch (_metric)
            {
                case EvaluateInfo.Metric.AUC:
                    info.auc = Auc(expectedProbs, predictedProbs);
                    break;
                case EvaluateInfo.Metric.LOGLOSS:
                    info.logloss = logloss(expectedProbs, predictedProbs);
                    break;
            }
            return info;
        }

        public EvaluateInfo Evaluate2(IBlackBox box, Dataset dataset)
        {
            EvaluateInfo info = new EvaluateInfo(_metric);
            List<double> predictedProbs = new List<double>(_indexes.Count);
            List<double> expectedProbs = new List<double>(_indexes.Count);
            for (int i = 0; i < _indexes.Count; i++)
            {
                var index = _indexes[i];
                DataRow dataRow = dataset.Samples[index];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs

                activate(box, inputs, outputs);
                predictedProbs.Add(outputs[0]);
                expectedProbs.Add(expected[0]);
                //calculateCorrectness(expected, outputs);
            }
            //File.WriteAllLines(@"predicted.txt", predictedProbs.Select(d => d.ToString()).ToArray());
            //FastAcyclicNetwork net = (FastAcyclicNetwork) box;
            //File.WriteAllLines(@"weights.txt", net.Connections.Select(d => d._weight.ToString()).ToArray());
            //info.accuracy = (double)correctlyClassified / samplesCount;
            //var temp = binarize(predictedProbs).Zip(expectedProbs, (x, y) => (int) x == (int) y ? 1.0 : 0.0);
            //info.accuracy = temp.Average();
            switch (_metric)
            {
                case EvaluateInfo.Metric.AUC:
                    info.auc = Auc(expectedProbs, predictedProbs);
                    break;
                case EvaluateInfo.Metric.LOGLOSS:
                    info.logloss = logloss(expectedProbs, predictedProbs);
                    break;
            }
            return info;
        }

        private double[] activateTest(IBlackBox box, IList<double> inputs, double[] outputs)
        {
            box.ResetState();
            var nodeId = 0;
            foreach (var input in inputs)
            {
                box.InputSignalArray[nodeId++] = input;
            }
            box.Activate();
            box.OutputSignalArray.CopyTo(outputs, 0);
            return outputs;
        }

        private double[] activate(IBlackBox box, IList<double> inputs, double[] outputs)
        {
            box.ResetState();
            var nodeId = 0;
            foreach (var input in inputs)
            {
                box.InputSignalArray[nodeId++] = input;
            }
            if (_overfittingParams.dropoutEnabled)
                box.ActivateWithDropout(_overfittingParams.dropoutInputP, _overfittingParams.dropoutHiddenP, _overfittingParams.triggerN);
            else box.Activate();
            box.OutputSignalArray.CopyTo(outputs, 0);        
            return outputs;
        }

        /// <summary>
        /// https://www.kaggle.com/wiki/LogarithmicLoss
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="predicted"></param>
        /// <returns></returns>
        public double logloss(List<double> actual, List<double> predicted)
        {
            double epsilon = 1e-15;
            double maxBound = 1 - epsilon;
            predicted = predicted.Select(x => Math.Max(epsilon, x)).ToList();
            predicted = predicted.Select(x => Math.Min(maxBound, x)).ToList();
            //predicted.ForEach(x => Math.Max(epsilon, x));
            //predicted.ForEach(x => Math.Min(maxBound, x));
            double ll = 0;
            //Debug.Assert(actual.);
            for (int i = 0; i < actual.Count; i++)
            {
                double fixedPred = predicted[i];
                //if (fixedPred <= 0) fixedPred = epsilon;
                //if (fixedPred == 1) fixedPred = 1 - epsilon;
                ll += actual[i]*Math.Log(fixedPred) + (1 - actual[i])*Math.Log(1 - fixedPred);
            }
            ll = ll*-1.0/actual.Count;
            return ll;
        }

        public static double Auc(List<double> a, List<double> p)
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

        private double[] binarize(List<double> values)
        {
            double[] result = new double[values.Count];
            for (int i = 0; i < values.Count; i++)
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
