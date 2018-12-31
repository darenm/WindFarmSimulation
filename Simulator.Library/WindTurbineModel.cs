using System;

namespace Simulator.Library
{
    public class WindTurbineModel
    {
        // Wind Turbine Power Calculator - http://xn--drmstrre-64ad.dk/wp-content/wind/miller/windpower%20web/en/tour/wres/pow/index.htm

        // http://xn--drmstrre-64ad.dk/wp-content/wind/miller/windpower%20web/en/tour/wtrb/comp/index.htm

        // http://www.wind-power-program.com/turbine_characteristics.htm

        // https://opendata-renewables.engie.com/pages/home/

        /*
         Parts of a Turbine
            1. Blades
            2. Rotor
            3. Blade Pitch
            4. Brake
            5. Low-speed shaft
            6. Gear box
            7. Generator
            8. Controller
            9. Anemometer
            10. Wind Vane
            11. Nacelle
            12. High-speed shaft
            13. Yaw drive
            14. Yaw motor
            15. Tower         */

        private const int UnBrakedLagSeconds = 480;
        private const int BrakedLagSeconds = 20;
        private const double LowSpeedShaftVariance = 0.02;

        private readonly VarianceDelayedDouble _lowSpeedShaftRpm = new VarianceDelayedDouble(0)
            {ValueLag = TimeSpan.FromSeconds(UnBrakedLagSeconds), Variance = LowSpeedShaftVariance};

        private bool _isTurbineBrakeOn = true;


        private double _windSpeed;

        /// <summary>
        ///     Wind Speed in Meters per Second m/s
        /// </summary>
        public double WindSpeed
        {
            get => _windSpeed;
            set
            {
                _windSpeed = value;
                UpdateWindSpeedDependencies();
            }
        }

        public double LowSpeedShaftRpm
        {
            get
            {
                _lowSpeedShaftRpm.Variance = IsTurbineBrakeOn ? 0 : LowSpeedShaftVariance;
                return _lowSpeedShaftRpm.Value;
            }
            private set => _lowSpeedShaftRpm.Value = value;
        }

        public double HighSpeedShaftRpm => LowSpeedShaftRpm * 105;

        public bool TriggerGeneratorOverTemp { get; set; }

        public double ExternalTemperatureCelsius { get; set; } = 22;

        /// <summary>
        /// Generator operating temp range - 0 - 50, warning 50-60 danger 60+
        /// </summary>
        public double GeneratorTemperatureCelsius => ExternalTemperatureCelsius + (Power / 20) + (TriggerGeneratorOverTemp ? 25 : 0) + VarianceGenerator.Generate(1);

        public double Power => LowSpeedShaftRpm * 20.0;

        public bool IsTurbineBrakeOn
        {
            get => _isTurbineBrakeOn;
            set
            {
                _lowSpeedShaftRpm.ValueLag = TimeSpan.FromSeconds(value ? BrakedLagSeconds : UnBrakedLagSeconds);
                _isTurbineBrakeOn = value;
            }
        }

        private void UpdateWindSpeedDependencies()
        {
            IsTurbineBrakeOn = WindSpeed < 5 || WindSpeed > 25;
            LowSpeedShaftRpm = IsTurbineBrakeOn ? 0 : PowerCurveModel.GetLowSpeedShaftRpm(WindSpeed);
        }
    }
}