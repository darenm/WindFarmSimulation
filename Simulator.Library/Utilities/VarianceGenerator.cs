using System;

namespace Simulator.Library.Utilities
{
    public static class VarianceGenerator
    {
        private static readonly Random CapturedRandom = new Random();

        public static double Generate(double variance)
        {
            // as random.Next requires an integer, multiply the variance by 100 so we can get some decimals
            // as random.next only returns a positive value - double the range and subtract the variance to get negative values
            var intVariance = Convert.ToInt32(variance * 100.0);
            var computedIntVariance = CapturedRandom.Next(2 * intVariance) - intVariance;
            var doubleVariance = Convert.ToDouble(computedIntVariance) / 100.0;
            return doubleVariance;
        }
    }
}