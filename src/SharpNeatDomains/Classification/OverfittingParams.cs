using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Domains.Classification
{
    public class OverfittingParams
    {
        public double dropoutInputP = 1;
        public double dropoutHiddenP = 0.5;
        public int triggerN = 3;

        public double subsample = 1;
        public bool interleaved;
        public double interleavedStartSubsample = 1;
        public double interleavedCrossSubsample = 0.2;
    }
}
