using Newtonsoft.Json;
using Simulator.Library.Dtos;

namespace WindFarmDashboard.Models
{
    public class DeviceMessage
    {
        [JsonProperty(PropertyName = "metadata")]
        public MetadataDto Metadata { get; set; }

        [JsonProperty(PropertyName = "telemetry")]
        public WindTurbineDto Telemetry { get; set; }
    }

    public class MetadataDto
    {
        [JsonProperty(PropertyName = "deviceType")]
        public string DeviceType { get; set; }

        [JsonProperty(PropertyName = "studentId")]
        public string StudentId { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }
    }
}