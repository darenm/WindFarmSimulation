using Newtonsoft.Json;

namespace Simulator.Library.Dtos
{
    public class WindTurbineDto
    {
        [JsonProperty(PropertyName = "windSpeed")]
        public double WindSpeed { get; set; }

        [JsonProperty(PropertyName = "lowSpeedShaftRpm")]
        public double LowSpeedShaftRpm { get; set; }

        [JsonProperty(PropertyName = "highSpeedShaftRpm")]
        public double HighSpeedShaftRpm { get; set; }

        [JsonIgnore]
        public bool TriggerGeneratorOverTemp { get; set; }

        [JsonProperty(PropertyName = "externalTemperatureCelsius")]
        public double ExternalTemperatureCelsius { get; set; }

        [JsonProperty(PropertyName = "generatorTemperatureCelsius")]
        public double GeneratorTemperatureCelsius { get; set; }

        [JsonIgnore]
        public bool TriggerRotorOverTemp { get; set; }

        [JsonProperty(PropertyName = "rotorTemperatureCelsius")]
        public double RotorTemperatureCelsius { get; set; }

        [JsonProperty(PropertyName = "power")]
        public double Power { get; set; }

        [JsonProperty(PropertyName = "isTurbineBrakeOn")]
        public bool IsTurbineBrakeOn { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}