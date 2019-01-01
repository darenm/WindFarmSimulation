using System;
using Newtonsoft.Json;
using Simulator.Library.Dtos;

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
            15. Tower 
        */

        private const int UnBrakedLagSeconds = 480;
        private const int BrakedLagSeconds = 20;
        private const double LowSpeedShaftVariance = 0.02;

        private readonly VarianceDelayedDouble _lowSpeedShaftRpm = new VarianceDelayedDouble(0)
            {ValueLag = TimeSpan.FromSeconds(UnBrakedLagSeconds), Variance = LowSpeedShaftVariance};

        private bool _isTurbineBrakeOn = true;


        private double _windSpeed;

        public string Name { get; set; }

        /// <summary>
        ///     Wind Speed in Meters per Second m/s
        /// </summary>
        public double WindSpeed
        {
            get => _windSpeed;
            set
            {
                _windSpeed = value + VarianceGenerator.Generate(3);
                UpdateWindSpeedDependencies();
            }
        }

        public double LowSpeedShaftRpm
        {
            get
            {
                _lowSpeedShaftRpm.Variance = IsTurbineBrakeOn || WindSpeed < 5 ? 0 : LowSpeedShaftVariance;
                return _lowSpeedShaftRpm.Value;
            }
            private set
            {
                _lowSpeedShaftRpm.Variance = IsTurbineBrakeOn || WindSpeed < 5 ? 0 : LowSpeedShaftVariance;
                _lowSpeedShaftRpm.Value = value;
            }
        }

        public double HighSpeedShaftRpm => LowSpeedShaftRpm * 105;

        public bool TriggerGeneratorOverTemp { get; set; } = false;

        public double ExternalTemperatureCelsius { get; set; } = 12;

        /// <summary>
        /// Generator operating temp range - 0 - 50, warning 50-60 danger 60+
        /// </summary>
        public double GeneratorTemperatureCelsius => ExternalTemperatureCelsius + (Power / 20) + (TriggerGeneratorOverTemp ? 25 : 0) + VarianceGenerator.Generate(1);

        public bool TriggerRotorOverTemp { get; set; } = false;

        /// <summary>
        /// Rotor operating temp range - 0 - 30, warning 30-35 danger 35+
        /// </summary>
        public double RotorTemperatureCelsius => ExternalTemperatureCelsius + (Power / 50) + (TriggerRotorOverTemp ? 25 : 0) + VarianceGenerator.Generate(1);

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
            IsTurbineBrakeOn = WindSpeed > 25;
            LowSpeedShaftRpm = IsTurbineBrakeOn || WindSpeed < 5 ? 0 : PowerCurveModel.GetLowSpeedShaftRpm(WindSpeed);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public WindTurbineDto ToDto()
        {
            return JsonConvert.DeserializeObject<WindTurbineDto>(ToJson());
        }
    }
}