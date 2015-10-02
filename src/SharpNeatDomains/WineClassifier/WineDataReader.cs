using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpNeat.Domains.WineClassifier
{
    class WineDataReader
    {
        public List<WineData> readWineData(string file)
        {
            List<WineData> data = new List<WineData>();
            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    WineData item = new WineData();
                    item.FixedAcidity = GetDouble(fields[0], 0);
                    item.VolatileAcidity = GetDouble(fields[1], 0);
                    item.CitricAcid = GetDouble(fields[2], 0);
                    item.ResidualSugar = GetDouble(fields[3], 0);
                    item.Chlorides = GetDouble(fields[4], 0);
                    item.FreeSulfurDioxide = GetDouble(fields[5], 0);
                    item.TotalSulfurDioxide = GetDouble(fields[6], 0);
                    item.Density = GetDouble(fields[7], 0);
                    item.PH = GetDouble(fields[8], 0);
                    item.Sulfates = GetDouble(fields[9], 0);
                    item.Alcohol = GetDouble(fields[10], 0);
                    item.Quality = GetDouble(fields[11], 0);
                    data.Add(item);
                }
            }
            return data;
        }

        private double GetDouble(string value, double defaultValue)
        {
            double result;

            //Try parsing in US english
            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then try in the current culture
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
