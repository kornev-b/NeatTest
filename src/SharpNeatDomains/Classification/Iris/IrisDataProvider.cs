using System.Collections.Generic;

namespace SharpNeat.Domains.Classification.Iris
{
    class IrisDataProvider : DataProvider
    {
        private static List<Dataset> cache;

        protected override string assertDelimeter()
        {
            return ";";
        }

        protected override string assertFileName()
        {
            return @"C:\Users\bkornev\Desktop\projects\NeatTest\src\datasets\iris\iris.train.fold.0.txt";
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
            var outputs = new List<double>(OutputsCount);
            
            for (int i = 0; i < fields.Length; i++)
            {
                double parsed = GetDouble(fields[i], 0);
                if (i < InputsCount)
                {
                    inputs.Add(parsed);
                }
                else
                {
                    outputs.Add(parsed);
                }

            }
            row.Inputs = inputs;
            row.Outputs = outputs;
            return row;
        }
    }
}
