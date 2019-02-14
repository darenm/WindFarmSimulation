namespace Simulator.Library.Models
{
    public static class PowerCurveModel
    {

        public static bool LowPowerOutput { get; set; }

        private static readonly PowerDatum[] KwValues =
        {
            new PowerDatum{Speed = 0, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 1, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 2, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 3, LowSpeedShaftRpm = 0.1,  Power = 2 },
            new PowerDatum{Speed = 4, LowSpeedShaftRpm = 0.85,  Power = 17 },
            new PowerDatum{Speed = 5, LowSpeedShaftRpm = 2.25,  Power = 45 },
            new PowerDatum{Speed = 6, LowSpeedShaftRpm = 3.6,  Power = 72 },
            new PowerDatum{Speed = 7, LowSpeedShaftRpm = 6.2,  Power = 124 },
            new PowerDatum{Speed = 8, LowSpeedShaftRpm = 9.8,  Power = 196 },
            new PowerDatum{Speed = 9, LowSpeedShaftRpm = 13.85,  Power = 277 },
            new PowerDatum{Speed = 10, LowSpeedShaftRpm = 18.2,  Power = 364 },
            new PowerDatum{Speed = 11, LowSpeedShaftRpm = 22.2,  Power = 444 },
            new PowerDatum{Speed = 12, LowSpeedShaftRpm = 26.65,  Power = 533 },
            new PowerDatum{Speed = 13, LowSpeedShaftRpm = 29.2,  Power = 584 },
            new PowerDatum{Speed = 14, LowSpeedShaftRpm = 30.9,  Power = 618 },
            new PowerDatum{Speed = 15, LowSpeedShaftRpm = 30.95,  Power = 619 },
            new PowerDatum{Speed = 16, LowSpeedShaftRpm = 30.9,  Power = 618 },
            new PowerDatum{Speed = 17, LowSpeedShaftRpm = 30.95,  Power = 619 },
            new PowerDatum{Speed = 18, LowSpeedShaftRpm = 31,  Power = 620 },
            new PowerDatum{Speed = 19, LowSpeedShaftRpm = 30.5,  Power = 610 },
            new PowerDatum{Speed = 20, LowSpeedShaftRpm = 29.7,  Power = 594 },
            new PowerDatum{Speed = 21, LowSpeedShaftRpm = 29.6,  Power = 592 },
            new PowerDatum{Speed = 22, LowSpeedShaftRpm = 29.5,  Power = 590 },
            new PowerDatum{Speed = 23, LowSpeedShaftRpm = 29,  Power = 580 },
            new PowerDatum{Speed = 24, LowSpeedShaftRpm = 28.75,  Power = 575 },
            new PowerDatum{Speed = 25, LowSpeedShaftRpm = 28.5,  Power = 570 },
            new PowerDatum{Speed = 26, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 27, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 28, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 29, LowSpeedShaftRpm = 0,  Power = 0 },
            new PowerDatum{Speed = 30, LowSpeedShaftRpm = 0,  Power = 0 },
        };

        /// <summary>
        ///     Returns the kW power produced for the supplied <see cref="windSpeed" /> in m/s
        /// </summary>
        /// <param name="windSpeed">Wind speed in meters per second</param>
        /// <returns>The power in kW</returns>
        public static double GetPower(double windSpeed)
        {
            var powerIndex = (int) windSpeed;
            if (powerIndex >= KwValues.Length || powerIndex <= 0) return 0; // the brake will be on!

            return KwValues[powerIndex].Power * (LowPowerOutput ? 0.9 : 1.0);
        }

        public static double GetLowSpeedShaftRpm(double windSpeed)
        {
            var powerIndex = (int)windSpeed;
            if (powerIndex >= KwValues.Length || powerIndex <= 0) return 0; // the brake will be on!

            return KwValues[powerIndex].LowSpeedShaftRpm;
        }

    }

    public struct PowerDatum
    {
        public double Speed { get; set; }
        public double LowSpeedShaftRpm { get; set; }
        public double Power { get; set; }
    }
}