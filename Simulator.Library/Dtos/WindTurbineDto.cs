namespace Simulator.Library.Dtos
{
    public class WindTurbineDto
    {
        public double WindSpeed { get; set; }
        public double LowSpeedShaftRpm { get; set; }
        public double HighSpeedShaftRpm { get; set; }
        public bool TriggerGeneratorOverTemp { get; set; }
        public double ExternalTemperatureCelsius { get; set; }
        public double GeneratorTemperatureCelsius { get; set; }
        public bool TriggerRotorOverTemp { get; set; }
        public double RotorTemperatureCelsius { get; set; }
        public double Power { get; set; }
        public bool IsTurbineBrakeOn { get; set; }
        public string Name { get; set; }
    }
}