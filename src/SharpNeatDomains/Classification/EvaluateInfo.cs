using System;

namespace SharpNeat.Domains.Classification
{
    /// <summary>
    ///             
    ///         -------------------------------------------------------------
    ///        |                     | Expected positive | Expected negative |          
    ///         -------------------------------------------------------------
    ///        |  Predicted positive |         TP        |        FP         |
    ///         -------------------------------------------------------------
    ///        |  Predicted negative |         FN        |        TN         |
    ///         -------------------------------------------------------------
    /// </summary>
    public class EvaluateInfo
    {
        public double auc;
        public double[][] probabilities;
        /// <summary>
        ///Predicted positive and expected positive
        /// </summary>
        public double TP;
        /// <summary>
        /// Predicted positive but expected negative 
        /// </summary>
        public double FP;
        /// <summary>
        ///Predicted negative but expected positive 
        /// </summary>
        public double FN;
        /// <summary>
        ///Predicted negative and expected negative
        /// </summary>
        public double TN;

        public int CorrectlyClassified;
        public int IncorrectlyClassified;

        private double accuracy;
        public double Accuracy
        {
            get
            {
                if (TP + TN + FP + FN > 0)
                {
                    accuracy = (TP + TN) / (TP + TN + FP + FN);
                }
                return accuracy;
            }
        }

        private double precision;
        /// <summary>
        ///Ratio of the number of correctly predicted positive (relevant)
        ///retrieved to the total number of incorrectly predicted (irrelevant)
        ///positive values and relevant values
        /// </summary>
        public double Precision
        {
            get
            {
                if (TP + FP > 0)
                {
                    precision = TP / (TP + FP);
                }
                return precision;
            }
        }
        /// <summary>
        ///Ratio of the number of correctly predicted positive (relevant)
        ///retrieved to the total number of incorrectly predicted (irrelevant)
        ///negative values and relevant values
        /// </summary>
        private double recall;
        public double Recall
        {
            get
            {
                if (TP + FN > 0)
                {
                    recall = TP / (TP + FN);
                }
                return recall;
            }
        }

        private double fMeasure;
        public double FMeasure
        {
            get
            {
                if (precision + recall > 0)
                {
                    fMeasure = 2 * (precision * recall / (precision + recall));
                }
                return fMeasure;
            }
        }

        public void Calculate()
        {
            fMeasure = FMeasure;
            precision = Precision;
            recall = Recall;
            accuracy = Accuracy;
        }
    }
}
