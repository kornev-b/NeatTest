using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.WineClassifier
{
    class WineDataNormalizer
    {
        public List<WineData> normalize(List<WineData> data)
        {
            double min = data.Min(x => x.FixedAcidity);
            double max = data.Max(x => x.FixedAcidity);
            double diff = max - min;
            foreach(WineData wine in data)
            {
                wine.FixedAcidity = (wine.FixedAcidity - min) / diff;
            }

            min = data.Min(x => x.Alcohol);
            max = data.Max(x => x.Alcohol);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.Alcohol = (wine.Alcohol - min) / diff;
            }

            min = data.Min(x => x.Chlorides);
            max = data.Max(x => x.Chlorides);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.Chlorides = (wine.Chlorides - min) / diff;
            }

            min = data.Min(x => x.CitricAcid);
            max = data.Max(x => x.CitricAcid);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.CitricAcid = (wine.CitricAcid - min) / diff;
            }

            min = data.Min(x => x.Density);
            max = data.Max(x => x.Density);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.Density = (wine.Density - min) / diff;
            }

            min = data.Min(x => x.FreeSulfurDioxide);
            max = data.Max(x => x.FreeSulfurDioxide);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.FreeSulfurDioxide = (wine.FreeSulfurDioxide - min) / diff;
            }

            min = data.Min(x => x.PH);
            max = data.Max(x => x.PH);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.PH = (wine.PH - min) / diff;
            }

            min = data.Min(x => x.ResidualSugar);
            max = data.Max(x => x.ResidualSugar);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.ResidualSugar = (wine.ResidualSugar - min) / diff;
            }

            min = data.Min(x => x.Sulfates);
            max = data.Max(x => x.Sulfates);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.Sulfates = (wine.Sulfates - min) / diff;
            }

            min = data.Min(x => x.TotalSulfurDioxide);
            max = data.Max(x => x.TotalSulfurDioxide);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.TotalSulfurDioxide = (wine.TotalSulfurDioxide - min) / diff;
            }

            min = data.Min(x => x.VolatileAcidity);
            max = data.Max(x => x.VolatileAcidity);
            diff = max - min;
            foreach (WineData wine in data)
            {
                wine.VolatileAcidity = (wine.VolatileAcidity - min) / diff;
            }

            return data;
        }
    }
}
