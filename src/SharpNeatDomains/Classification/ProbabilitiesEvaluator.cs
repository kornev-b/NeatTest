using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;

namespace SharpNeat.Domains.Classification
{
    public class ProbabilitiesEvaluator
    {
        public double[][] predictProba(IBlackBox box, Dataset dataset)
        {
            int samplesCount = dataset.Samples.Count;

            double[][] proba = new double[dataset.Samples.Count][];

            for (int i = 0; i < samplesCount; i++)
            {
                DataRow dataRow = dataset.Samples[i];
                var inputs = dataRow.Inputs;
                var expected = dataRow.Outputs;
                var outputs = new double[dataset.OutputCount];
                // activate our black box and get outputs
                activate(box, inputs, outputs);
                proba[i] = outputs;
            }

            return proba;
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
    }
}
