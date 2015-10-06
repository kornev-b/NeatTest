using System.Collections.Generic;
using SharpNeat.Domains.Mine;

namespace SharpNeat.Domains.Classification.Iris
{
    class IrisDataProvider : DataProvider
    {
        private static List<Dataset> cache;

        protected override string assertDelimeter()
        {
            return ",";
        }

        protected override string assertFileName()
        {
            return @"K:\nn\SharpNeat\src\iris.data.txt";
        }

        protected override int assertInputsCount()
        {
            return 4;
        }

        protected override int assertOutputsCount()
        {
            return 3;
        }

        protected override DataRow parseDataRow(string[] fields)
        {
            DataRow row = new DataRow();
            var inputs = new List<double>(InputsCount);
            
            for (int i = 0; i < InputsCount; i++)
            {
                double parsed = GetDouble(fields[i], 0);
                inputs.Add(parsed);
            }
            row.Inputs = inputs;
            double output = GetDoubleOutput(fields[fields.Length - 1], 0);
            var outputs = ConvertToBinaryOrderedList(output);
            row.Outputs = outputs;
            return row;
        }

        private double GetDoubleOutput(string text, int defaultValue)
        {
            if (text.Equals("Iris-setosa"))
            {
                return 0;
            }
            if (text.Equals("Iris-versicolor"))
            {
                return 1;
            }
            if (text.Equals("Iris-virginica"))
            {
                return 2;
            }
            return defaultValue;
        }
    }
}
