using SharpNeat.Genomes.Neat;
using System.IO;
using SharpNeat.Decoders;
using SharpNeat.Phenomes.NeuralNets;
using System.Text;

namespace SharpNeatGUI
{
    class SimpleGenomeSaver
    {
        public static void saveGenome(string filename, NeatGenome neatGenome)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                FastAcyclicNetwork network = FastAcyclicNetworkFactory.CreateFastAcyclicNetwork(neatGenome);
                writer.WriteLine(network.NodesCount);
                writer.WriteLine(network.InputCount);
                writer.WriteLine(network.OutputCount);
                writer.WriteLine(network.LayersInfo.Length);
                StringBuilder sbWeights = new StringBuilder();
                StringBuilder sbSrcConns = new StringBuilder();
                StringBuilder sbTgtConns = new StringBuilder();
                foreach(FastConnection con in network.Connections)
                {
                    sbWeights.Append(con._weight).Append(" ");
                    sbSrcConns.Append(con._srcNeuronIdx).Append(" ");
                    sbTgtConns.Append(con._tgtNeuronIdx).Append(" ");                 
                }
                if(sbWeights.Length > 1)
                    sbWeights.Remove(sbWeights.Length - 1, 1);
                if(sbSrcConns.Length > 1)
                    sbSrcConns.Remove(sbSrcConns.Length - 1, 1);
                if (sbTgtConns.Length > 1)
                    sbTgtConns.Remove(sbTgtConns.Length - 1 , 1);
                writer.WriteLine(sbWeights.ToString());
                writer.WriteLine(sbSrcConns.ToString());
                writer.WriteLine(sbTgtConns.ToString());
                StringBuilder sbLayersConnsInfo = new StringBuilder();
                StringBuilder sbLayersNodesInfo = new StringBuilder();
                foreach (LayerInfo layerInfo in network.LayersInfo)
                {
                    sbLayersConnsInfo.Append(layerInfo._endConnectionIdx).Append(" ");
                    sbLayersNodesInfo.Append(layerInfo._endNodeIdx).Append(" ");
                }
                if (sbLayersConnsInfo.Length > 1)
                    sbLayersConnsInfo.Remove(sbLayersConnsInfo.Length - 1, 1);
                if (sbLayersNodesInfo.Length > 1)
                    sbLayersNodesInfo.Remove(sbLayersNodesInfo.Length - 1, 1);
                writer.WriteLine(sbLayersConnsInfo);
                writer.WriteLine(sbLayersNodesInfo);
                StringBuilder sbOutputsIndexes = new StringBuilder();
                foreach(int output in network.OutputIdxArray)
                {
                    sbOutputsIndexes.Append(output).Append(" ");
                }
                if (sbOutputsIndexes.Length > 1)
                    sbOutputsIndexes.Remove(sbOutputsIndexes.Length - 1, 1);
                writer.WriteLine(sbOutputsIndexes);
            }
        }
    }
}
