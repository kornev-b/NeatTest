using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNeat.Domains.WineClassifier
{
    public class WineBlackBoxEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        const double StopFitness = 10.0;
        const int AcceptedAccuracy = 70; // in percent
        ulong _evalCount;
        bool _stopConditionSatisfied;

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public FitnessInfo Evaluate(IBlackBox box)
        {
            double accuracy = 0;
            double responseCluster;
            int clustersCount = 10;

            ISignalArray inputArr = box.InputSignalArray;
            ISignalArray outputArr = box.OutputSignalArray;
            List<WineData> wineData = WineDataProvider.getWineData();
            int[] clusters = new int[clustersCount];
            int[] correctGuessedClusters = new int[clustersCount];
            int count = wineData.Count > 4000 ? 4000 : wineData.Count;
            for (int i = 0; i < count; i++)
            {
                //if(_evalCount++ > 1000)
                //{
                //    break;
                //}
                WineData data = wineData[i];
                box.ResetState();
                // Set the input values
                inputArr[0] = data.FixedAcidity;
                inputArr[1] = data.VolatileAcidity;
                inputArr[2] = data.CitricAcid;
                inputArr[3] = data.ResidualSugar;
                inputArr[4] = data.Chlorides;
                inputArr[5] = data.FreeSulfurDioxide;
                inputArr[6] = data.TotalSulfurDioxide;
                inputArr[7] = data.Density;
                inputArr[8] = data.PH;
                inputArr[9] = data.Sulfates;
                inputArr[10] = data.Alcohol;
                // increase cluster members count
                double correctCluster = data.Quality;
                clusters[Convert.ToInt32(correctCluster)]++;
                // Activate the black box.
                box.Activate();
                if (!box.IsStateValid)
                {   // Any black box that gets itself into an invalid state is unlikely to be
                    // any good, so lets just bail out here.
                    return FitnessInfo.Zero;
                }

                // Read output signal.
                responseCluster = outputArr[0];
                Debug.Assert(responseCluster >= 0.0, "Unexpected negative output.");
                int guessedCluster = Convert.ToInt32(responseCluster);
                if(guessedCluster >= 0 && guessedCluster <= 9)
                {
                    if(data.Quality == guessedCluster)
                    {
                        Debug.WriteLine("Yesss! One claster has been guessed correctly!");
                        correctGuessedClusters[guessedCluster]++;
                    }
                }              
            }
            int actualClustersNumber = getNumberOfActualClusters(clusters);
            if(actualClustersNumber == 0)
            {
                //Debug.Assert(actualClustersNumber >= 0.0, "Unexpected clusters count: 0");
                return FitnessInfo.Zero;
            }
            
            for(int i = 0; i < clustersCount; i++)
            {
                if(clusters[i] == 0)
                {
                    continue;
                }
                accuracy += correctGuessedClusters[i] / clusters[i];
            }

            accuracy /= clustersCount;
            accuracy *= 100; // in percent
            //Debug.WriteLine("Accuracy = " + accuracy + "%");

                // Note. This is correct. Network's response is subtracted from MaxError; if all responses are correct then fitness == MaxError.
            if (accuracy >= AcceptedAccuracy)
            {
                _stopConditionSatisfied = true;
            }

            return new FitnessInfo(accuracy, accuracy);
        }

        private int getNumberOfActualClusters(int[] clusters)
        {
            int count = 0;
            foreach(int cluster in clusters)
            {
                if(cluster > 0)
                {
                    count++;
                }
            }
            return count;
        }

        public void Reset()
        {
        }
    }
}
