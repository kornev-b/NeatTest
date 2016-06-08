using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification.Digits
{
    public class DigitsDataProvider : DataProvider
    {
        protected override DataRow parseDataRow(string[] fields)
        {
            DataRow row = new DataRow();
            for (int i = 0; i < assertInputsCount(); i++)
            {
                double value = GetDouble(fields[i], 0);
                row.Inputs.Add(value);
            }
            for (int i = 0; i < assertOutputsCount(); i++)
            {
                row.Outputs.Add(0);
            }
            row.Outputs[(int) GetDouble(fields[fields.Length - 1], 0)] = 1;
            return row;
        }

        protected override string assertDelimeter() => ",";

        protected override int assertInputsCount() => 784;

        protected override int assertOutputsCount() => 10;

        protected override string assertFileName() => @"..\..\..\..\datasets\digits\digits_train.csv";

        protected override string assertValidationFileName() => @"..\..\..\..\datasets\digits\digits_test.csv";
    }
}
