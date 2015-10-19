using System.Collections.Generic;

namespace SharpNeat.Domains
{
    public class DataRow
    {
        public List<double> Inputs;
        public List<double> Outputs;

        public DataRow()
        {
            Inputs = new List<double>();
            Outputs = new List<double>();
        }
    }
}
