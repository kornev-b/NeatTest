using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification.Taxi
{
    public class TaxiDataProvider : DataProvider
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

        protected override string assertDelimeter() => ",";

        protected override int assertInputsCount() => 24;

        protected override int assertOutputsCount() => 1;

        protected override string assertFileName() => @"..\..\..\..\datasets\taxi\taxi-train.csv";

        protected override string assertValidationFileName() => @"..\..\..\..\datasets\taxi\taxi-test.csv";
    }
}
