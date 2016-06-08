using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System.Threading.Tasks;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains.Classification.Adult;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Network;
using SharpNeat.Network.ActivationFunctions.Bipolar;
using SharpNeat.SpeciationStrategies;

namespace SharpNeat.Domains.Classification.Digits
{
    class DigitsExperiment : OverfittingExperiment
    {
        private const int SEED = 24;
        NeatEvolutionAlgorithmParameters _eaParams;
        NeatGenomeParameters _neatGenomeParams;
        string _name;
        int _populationSize;
        int _specieCount;
        NetworkActivationScheme _activationScheme;
        string _complexityRegulationStr;
        int? _complexityThreshold;
        string _description;
        ParallelOptions _parallelOptions;
        Fitness _fitness = Fitness.FMEASURE;
        string _trainFilePath;
        private OverfittingParams _overfittingParams = new OverfittingParams();

        public OverfittingParams OverfittingParams
        {
            get { return _overfittingParams; }
        }

        public int DefaultPopulationSize
        {
            get { return _populationSize; }
        }

        public string Description
        {
            get { return _description; }
        }

        public int InputCount
        {
            get { return 784; }
        }

        public string Name
        {
            get { return _name; }
        }

        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
        {
            get { return _eaParams; }
        }

        public NeatGenomeParameters NeatGenomeParameters
        {
            get { return _neatGenomeParams; }
        }

        public int OutputCount
        {
            get { return 10; }
        }

        public void Initialize(string name, XmlElement xmlConfig)
        {
            _name = name;
            _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
            _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
            _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
            _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);

            _eaParams = new NeatEvolutionAlgorithmParameters();
            _eaParams.SpecieCount = _specieCount;
            _neatGenomeParams = new NeatGenomeParameters();
            _neatGenomeParams.ActivationFn = PlainSigmoid.__DefaultInstance;
            _neatGenomeParams.HiddenUnitActivationFn = ReLU.__DefaultInstance;
            _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
        }

        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        public AbstractDomainView CreateDomainView()
        {
            return null;
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            return CreateEvolutionAlgorithm(_populationSize);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
        {
            // Create a genome factory with our neat genome parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithm(genomeFactory, genomeList);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create IBlackBox evaluator.
            DigitsBlackBoxEvaluator evaluator = new DigitsBlackBoxEvaluator();
            DigitsDataProvider dataProvider = new DigitsDataProvider();
            evaluator.DataProvider = dataProvider;
            evaluator.Fitness = _fitness;
            evaluator._overfittingParams = _overfittingParams;
            dataProvider.getData();
            dataProvider.getEvalData();
            // Create the evolution algorithm.
            DgiitsNeatEvolutionAlgorithm ea = new DgiitsNeatEvolutionAlgorithm(dataProvider,_eaParams, speciationStrategy, complexityRegulationStrategy);
            //ea.Seed(SEED);
            File.WriteAllText("na_see.txt", "" + ea.UsedSeed);
            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            DigitsGenomeListEvaluator<NeatGenome> innerEvaluator = 
                //new ParallelGenomeListEvaluator<NeatGenome,IBlackBox>(genomeDecoder, evaluator, _parallelOptions, true);
                new DigitsGenomeListEvaluator<NeatGenome>(genomeDecoder, evaluator, _parallelOptions, true, dataProvider);
            innerEvaluator._overfittingParams = _overfittingParams;

            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determined by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(
                                                                                    innerEvaluator,
                                                                                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());
            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_activationScheme);
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            DigitsNeatGenomeFactory factory = new DigitsNeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
            //factory.Seed(SEED);
            File.WriteAllText("seed_facroty.txt", ""+factory.Rng.seed);
            return factory;
        }

        public AbstractGenomeView CreateGenomeView()
        {
            return new NeatGenomeView();
        }

        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
        }
    }
}
