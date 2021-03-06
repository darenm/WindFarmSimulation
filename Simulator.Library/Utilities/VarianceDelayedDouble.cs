﻿using System;

namespace Simulator.Library.Utilities
{
    public class VarianceDelayedDouble : DelayedDouble
    {
        public VarianceDelayedDouble(double actualValue) : base(actualValue)
        {
        }

        public double Variance { get; set; }

        public bool CanValueBeNegative { get; set; }

        public override double Value
        {
            get
            {
                if (Math.Abs(Variance) < 0.0001) return base.Value;

                var returnValue = base.Value + VarianceGenerator.Generate(Variance);

                return CanValueBeNegative ? returnValue : returnValue < 0 ? 0 : returnValue ;
            }
            set => base.Value = value;
        }
    }
}