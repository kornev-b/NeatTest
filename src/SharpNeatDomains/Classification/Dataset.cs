using System.Collections.Generic;

namespace SharpNeat.Domains.Classification
{
    public class Dataset
    {
        public List<DataRow> Samples;
        public int InputCount;
        public int OutputCount;

        public Dataset()
        {
            Samples = new List<DataRow>();
        }

        public void Add(DataRow row)
        {
            Samples.Add(row);
        }
    }
}
