using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Utility;
using Accord.Statistics.Distributions.Univariate;

namespace SharpNeat.Phenomes.NeuralNets
{
    public class BernoulliRandomizer
    {
        private static BernoulliDistribution bernoulli05 = new BernoulliDistribution(0.5);
        private static BernoulliDistribution bernoulli08 = new BernoulliDistribution(0.8);

        public static int NextP05()
        {
            return bernoulli05.Generate();
        }

        public static int NextP08()
        {
            return bernoulli08.Generate();
        }

        public static int[] Next(int n, double p)
        {
            return new BernoulliDistribution(p).Generate(n);
        }

        public static int[] NextP05(int n)
        {
            return bernoulli05.Generate(n);
        }

        public static int[] NextP08(int n)
        {
            return bernoulli08.Generate(n);
        }
    }
}
