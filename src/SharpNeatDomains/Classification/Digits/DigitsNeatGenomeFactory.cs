using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Domains.Classification.Digits
{
    public class DigitsNeatGenomeFactory : NeatGenomeFactory
    {
        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount) : base(inputNeuronCount, outputNeuronCount)
        {
        }

        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, NeatGenomeParameters neatGenomeParams) : base(inputNeuronCount, outputNeuronCount, neatGenomeParams)
        {
        }

        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, NeatGenomeParameters neatGenomeParams, UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator) : base(inputNeuronCount, outputNeuronCount, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary)
        {
        }

        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams)
        {
        }

        public DigitsNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams, UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        public void Seed(int seed)
        {
            _rng.Reinitialise(seed);
            _gaussianSampler = new ZigguratGaussianSampler(seed);
        }
    }
}
