using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Utility;

namespace SharpNeat.Network.ActivationFunctions.Bipolar
{
    public class ReLU : IActivationFunction
    {
        public static readonly IActivationFunction __DefaultInstance = new ReLU();

        public string FunctionId { get { return this.GetType().Name; } }
        public string FunctionString { get { return "y = max(0,x)"; } }
        public string FunctionDescription { get
        {
            return "The rectifier is, as of 2015, the most popular activation function for deep neural networks.";
        } }
        public bool AcceptsAuxArgs { get { return false; } }
        public double Calculate(double x, double[] auxArgs)
        {
            if (x > 0) return x;
            return 0;
        }

        public float Calculate(float x, float[] auxArgs)
        {
            if (x > 0) return x;
            return 0;
        }

        public double[] GetRandomAuxArgs(FastRandom rng, double connectionWeightRange)
        {
            throw new SharpNeatException("GetRandomAuxArgs() called on activation function that does not use auxiliary arguments.");
        }

        public void MutateAuxArgs(double[] auxArgs, FastRandom rng, ZigguratGaussianSampler gaussianSampler,
            double connectionWeightRange)
        {
            throw new SharpNeatException("MutateAuxArgs() called on activation function that does not use auxiliary arguments.");
        }
    }
}
