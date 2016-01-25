using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification.Adult
{
    class AdultDataProvider : DataProvider
    {

        public string Filename { get; set; }

        protected override string assertDelimeter()
        {
            return " ";
        }

        protected override string assertFileName()
        {
            return @Filename;
        }

        protected override int assertInputsCount()
        {
            return 13;
        }

        protected override int assertOutputsCount()
        {
            return 2;
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
