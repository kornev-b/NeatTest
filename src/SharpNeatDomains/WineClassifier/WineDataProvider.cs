using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.WineClassifier
{
    class WineDataProvider
    {
        private static List<WineData> cache;

        public static List<WineData> getWineData()
        {
            if(cache == null)
            {
                cache = new WineDataReader().readWineData("K:\\nn\\SharpNeat\\src\\winequality-white.csv");
                cache = new WineDataNormalizer().normalize(cache);
            }
            return cache;
        }
    }
}
