using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Domains.Classification.Koby
{
    public class KobyNeatGenomeFactory : NeatGenomeFactory
    {
        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount) : base(inputNeuronCount, outputNeuronCount)
        {
        }

        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, NeatGenomeParameters neatGenomeParams) : base(inputNeuronCount, outputNeuronCount, neatGenomeParams)
        {
        }

        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, NeatGenomeParameters neatGenomeParams, UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator) : base(inputNeuronCount, outputNeuronCount, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary)
        {
        }

        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams)
        {
        }

        public KobyNeatGenomeFactory(int inputNeuronCount, int outputNeuronCount, IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams, UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator) : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeIdGenerator, innovationIdGenerator)
        {
        }

        public void Seed(int seed)
        {
            _rng.Reinitialise(seed);
            _gaussianSampler = new ZigguratGaussianSampler(seed);
        }
    }
}
