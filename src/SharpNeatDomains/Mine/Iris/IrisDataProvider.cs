using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Mine.Iris
{
    class IrisDataProvider
    {
        private static List<Iris> cache;

        public static List<Iris> getIrisData()
        {
            if (cache == null)
            {
                cache = new IrisDataReader().readIrisData(@"K:\nn\SharpNeat\src\iris.data.txt");
                //cache = new IrisDataNormalizer().normalize(cache);
            }
            return cache;
        }
    }
}
