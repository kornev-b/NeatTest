﻿using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace SharpNeat.Domains.Classification.Iris
{
    class IrisDataReader
    {
        public List<Mine.Iris.Iris> readIrisData(string file)
        {
            List<Mine.Iris.Iris> data = new List<Mine.Iris.Iris>();
            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    Mine.Iris.Iris item = new Mine.Iris.Iris();
                    item.SepalLength = GetDouble(fields[0], 0);
                    item.SepalWidth = GetDouble(fields[1], 0);
                    item.PetalLength = GetDouble(fields[2], 0);
                    item.PetalWidth = GetDouble(fields[3], 0);
                    item.cluster = GetCluster(fields[4], 1);
                    data.Add(item);
                }
            }
            return data;
        }

        private int GetCluster(string name, int defaultCluster)
        {
            if (name.Equals("Iris-setosa"))
            {
                return 0;
            }
            if (name.Equals("Iris-versicolor"))
            {
                return 1;
            }
            if (name.Equals("Iris-virginica"))
            {
                return 2;
            }
            return defaultCluster;
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
