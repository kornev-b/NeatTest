using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Domains;
using SharpNeat.Domains.Classification;

namespace CrossValidation
{
    class CrossValidationDatasetProvider : DataProvider
    {
        private string _filename;
        private int _inputsCount;
        private int _outputsCount;
        private string _delimeter;

        private CrossValidationDatasetProvider(string filename, int inputsCount, 
            int outputsCount, string delimeter)
        {
            _filename = filename;
            _inputsCount = inputsCount;
            _outputsCount = outputsCount;
            _delimeter = delimeter;
            trainCache = null;
        }


        public class Builder
        {
            private string _filename;
            private int _inputsCount;
            private int _outputsCount;
            private string _delimeter;

            public Builder filename(String filename)
            {
                _filename = filename;
                return this;
            }

            public Builder inputsCount(int inputsCount)
            {
                _inputsCount = inputsCount;
                return this;
            }

            public Builder outputsCount(int outputsCount)
            {
                _outputsCount = outputsCount;
                return this;
            }

            public Builder delimeter(string delimeter)
            {
                _delimeter = delimeter;
                return this;
            }

            public CrossValidationDatasetProvider build()
            {
                return new CrossValidationDatasetProvider(_filename, _inputsCount, 
                    _outputsCount, _delimeter);
            }
        }

        protected override DataRow parseDataRow(string[] fields)
        {
            DataRow row = new DataRow();
            for (int i = 0; i < fields.Length; i++)
            {
                if (i < _inputsCount)
                {
                    row.Inputs.Add(GetDouble(fields[i], 0));
                }
                else
                {
                    row.Outputs.Add(GetDouble(fields[i], 0));
                }
            }
            if (row.Outputs.Count < _outputsCount)
            {
                return null;
            }
            return row;
        }

        protected override string assertDelimeter()
        {
            return _delimeter;
        }

        protected override int assertInputsCount()
        {
            return _inputsCount;
        }

        protected override int assertOutputsCount()
        {
            return _outputsCount;
        }

        protected override string assertFileName()
        {
            return _filename;
        }
    }
}
