using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification.dota2
{
    class Dota2DataProvider : DataProvider
    {
        protected override DataRow parseDataRow(string[] fields)
        {
            DataRow row = new DataRow();
            for (int i = 0; i < assertInputsCount(); i++)
            {
                double value = GetDouble(fields[i], 0);
                row.Inputs.Add(value);
            }
            for (int i = assertInputsCount(); i < fields.Length; i++)
            {
                row.Outputs.Add(GetDouble(fields[i], 0));
            }
            return row;
        }

        protected override string assertDelimeter()
        {
            return ",";
        }

        protected override int assertInputsCount()
        {
            return 210;
        }

        protected override int assertOutputsCount()
        {
            return 2;
        }

        protected override string assertFileName()
        {
            return @"K:\nn\SharpNeat\NeatTest\src\datasets\clean_dota2_train";
        }
    }
}
