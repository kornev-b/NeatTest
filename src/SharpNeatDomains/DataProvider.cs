using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using SharpNeat.Domains.Classification;

namespace SharpNeat.Domains
{
    public abstract class DataProvider
    {
        private volatile int ms;
        protected int InputsCount { get; set; }
        protected int OutputsCount { get; set; }

        protected static Dataset trainCache;
        protected static Dataset evalCache;

        public DataProvider()
        {
            InputsCount = assertInputsCount();
            OutputsCount = assertOutputsCount();
        }

        public virtual Dataset getData()
        {
            if (trainCache != null)
            {
                return trainCache;
            }
            trainCache = new Dataset();
            trainCache.InputCount = assertInputsCount();
            trainCache.OutputCount = assertOutputsCount();
            using (TextFieldParser parser = new TextFieldParser(assertFileName()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(assertDelimeter());
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    DataRow dataRow = parseDataRow(fields);
                    if (dataRow == null)
                    {
                        continue;
                    }
                    trainCache.Add(dataRow);
                }
            }
            return trainCache;
        }

        protected abstract DataRow parseDataRow(string[] fields);

        protected abstract string assertDelimeter();

        protected abstract int assertInputsCount();

        protected abstract int assertOutputsCount();

        protected abstract string assertFileName();

        protected virtual string assertValidationFileName()
        {
            return "";
        }

        protected double GetDouble(string value, double defaultValue)
        {
            double result;

            //Try parsing in US english
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then try in the current culture
                !double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then in neutral language
                !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
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

        public virtual Dataset getEvalData()
        {
            if (evalCache != null)
            {
                return evalCache;
            }
            evalCache = new Dataset();
            evalCache.InputCount = assertInputsCount();
            evalCache.OutputCount = assertOutputsCount();
            using (TextFieldParser parser = new TextFieldParser(assertValidationFileName()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(assertDelimeter());
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    DataRow dataRow = parseDataRow(fields);
                    if (dataRow == null)
                    {
                        continue;
                    }
                    evalCache.Add(dataRow);
                }
            }
            return evalCache;
        }
    }
}
