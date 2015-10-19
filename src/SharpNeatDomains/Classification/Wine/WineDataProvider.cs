using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification.Wine
{
    class WineDataProvider : DataProvider
    {
        private static Dataset cache;

        public new Dataset getData()
        {
            if (cache == null)
            {
                cache = base.getData();
            }
            return cache;
        }

        protected override DataRow parseDataRow(string[] fields)
        {
            DataRow dataRow = new DataRow();
            for (int i = 0; i < fields.Length; i++)
            {
                if (i < InputsCount)
                {
                    dataRow.Inputs.Add(GetDouble(fields[i], 0));
                }
                else
                {
                    dataRow.Outputs.Add(GetDouble(fields[i], 0));
                }
            }
            return dataRow;
        }

        protected override string assertDelimeter()
        {
            return ";";
        }

        protected override int assertInputsCount()
        {
            return 11;
        }

        protected override int assertOutputsCount()
        {
            return 10;
        }

        protected override string assertFileName()
        {
            return @"K:\nn\SharpNeat\NeatTest\src\parsed.data.wine.txt";
        }
    }
}
