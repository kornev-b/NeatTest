using System.Collections.Generic;
using System.Linq;

namespace SharpNeat.Domains.Classification.Iris
{
    class IrisDataNormalizer
    {
        public List<Mine.Iris.Iris> normalize(List<Mine.Iris.Iris> data)
        {
            double min = data.Min(x => x.SepalLength);
            double max = data.Max(x => x.SepalLength);
            double diff = max - min;
            foreach (Mine.Iris.Iris iris in data)
            {
                iris.SepalLength = (iris.SepalLength - min) / diff;
            }
            min = data.Min(x => x.SepalWidth);
            max = data.Max(x => x.SepalWidth);
            diff = max - min;
            foreach (Mine.Iris.Iris iris in data)
            {
                iris.SepalWidth = (iris.SepalWidth - min) / diff;
            }
            min = data.Min(x => x.PetalLength);
            max = data.Max(x => x.PetalLength);
            diff = max - min;
            foreach (Mine.Iris.Iris iris in data)
            {
                iris.PetalLength = (iris.PetalLength - min) / diff;
            }
            min = data.Min(x => x.PetalWidth);
            max = data.Max(x => x.PetalWidth);
            diff = max - min;
            foreach (Mine.Iris.Iris iris in data)
            {
                iris.PetalWidth = (iris.PetalWidth - min) / diff;
            }
            return data;
        }
    }
}
