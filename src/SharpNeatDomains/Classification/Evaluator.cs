using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification
{
    public class Evaluator
    {
        int correctlyClassified;
        int incorrectlyClassified;
        //double[][] predicions;
        int punishment = 1;

        public enum ResultType
        {
            TP, TN, FP, FN
        }

        public EvaluateInfo Evaluate(IBlackBox box, Dataset dataset)
        {
            EvaluateInfo info = new EvaluateInfo();

            int samplesCount = dataset.Samples.Count;
            var results = new ResultType[samplesCount][];
            correctlyClassified = 0;
            incorrectlyClassified = 0;
            //predicions = new double[samplesCount][];

            for (int i = 0; i < samplesCount; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs
                activate(box, inputs, outputs);
                calculateCorrectness(expected, outputs);
                //predicions[i] = outputs;
                // apply to each pair of outputs array element and expected array element
                // a function that is determined what kind of ResultType
                // this pair produces (TP, TN, FN, TN)
                results[i] = outputs.Zip(expected, (o, e) => getResultType(o, e)).ToArray();
                Debug.Print("Predicted: " + outputs[0]);
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


            var TP = TPs.Average();
            var TN = TNs.Average();
            var FP = FPs.Average();
            var FN = FNs.Average();

            info.CorrectlyClassified = correctlyClassified;
            info.IncorrectlyClassified = incorrectlyClassified;
            info.TP = TP;
            info.TN = TN;
            info.FP = FP;
            info.FN = FN;
            
            info.Calculate();

            return info;
        }

        /*private void calcPunishment()
        {
            double[] first = predicions[0];
            bool shouldPunish = predicions.All(x => equals(first, x));
            punishment = shouldPunish ? 1 : 0;
        }*/
        

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

        /// <summary>
        /// Interpret a result as a true positive, false positive, true negative or false negative.
        /// </summary>
        private ResultType getResultType(double output, double expected)
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
            return isCorrect ? ResultType.TN : ResultType.FN;
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
    }
}
