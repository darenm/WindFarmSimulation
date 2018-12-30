namespace Simulator.Library
{
    public static class PowerCurveModel
    {
        private static readonly double[] KwValues =
        {
            0, 0, 0, 2, 17, 45, 72, 124, 196, 277, 364, 444, 533, 584, 618, 619, 618, 619, 620, 610, 594, 592, 590, 580,
            575, 570, 0, 0, 0, 0, 0
        };

        /// <summary>
        ///     Returns the kW power produced for the supplied <see cref="windSpeed" /> in m/s
        /// </summary>
        /// <param name="windSpeed">Wind speed in meters per second</param>
        /// <returns>The power in kW</returns>
        public static double GetPower(double windSpeed)
        {
            var powerIndex = (int) windSpeed;
            if (powerIndex >= KwValues.Length || powerIndex == 0) return 0; // the brake will be on!

            return KwValues[powerIndex];
        }
    }
}