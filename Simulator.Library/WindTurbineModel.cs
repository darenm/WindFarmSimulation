namespace Simulator.Library
{
    public class WindTurbineModel
    {
        // Wind Turbine Power Calculator - http://xn--drmstrre-64ad.dk/wp-content/wind/miller/windpower%20web/en/tour/wres/pow/index.htm

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

        private const double 

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

        public bool IsTurbineBrakeOn { get; set; }

        private void UpdateWindSpeedDependencies()
        {
            IsTurbineBrakeOn = WindSpeed < 5 || WindSpeed > 25;
        }
    }
}