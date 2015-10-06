using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace SharpNeat.Domains.Mine
{
    abstract class DataProvider
    {
        protected int InputsCount { get; set; }
        protected int OutputsCount { get; set; }

        public DataProvider()
        {
            InputsCount = assertInputsCount();
            OutputsCount = assertOutputsCount();
        }

        public Dataset getData()
        {
            var data = new Dataset();
            data.InputCount = assertInputsCount();
            data.OutputCount = assertOutputsCount();
            using (TextFieldParser parser = new TextFieldParser(assertFileName()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(assertDelimeter());
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    DataRow dataRow = parseDataRow(fields);
                    data.Add(dataRow);
                }
            }
            return data;
        }

        protected abstract DataRow parseDataRow(string[] fields);

        protected abstract string assertDelimeter();

        protected abstract int assertInputsCount();

        protected abstract int assertOutputsCount();

        protected abstract string assertFileName();

        protected double GetDouble(string value, double defaultValue)
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

        /// <summary>
        /// if outputsCount = 3: 0 -> 001; 1 -> 010; 2 -> 100; etc
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        protected List<double> ConvertToBinaryOrderedList(double output)
        {
            int number = Convert.ToInt32(output);
            var result = new List<double>();
            for (int i = OutputsCount - 1; i >=0; i--)
            {
                if (i != number)
                {
                    result.Add(0);
                }
                else
                {
                    result.Add(1);
                }
            }
            return result;
        }
    }
}
