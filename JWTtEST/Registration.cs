using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTtEST
{
    public class Registration
    {
        [JsonIgnore]
        public long Id { get; set; }
        [ForeignKey("Device")]
        [JsonIgnore]
        public long DeviceId { get; set; }
        public virtual Device Device { get; set; }
        public double Created { get; set; }
        public string CreatedAsString { get; set; }
        public string Description { get; set; }
        [JsonProperty("Id")]
        public string DeviceEnpointId { get; set; }
        public bool Enabled { get; set; }
        public string HardwareId { get; set; }
        public string HardwareRevision { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        [JsonIgnore]
        public string SharedSecret { get; set; }
        [JsonProperty("SharedSecret")]
        public string SharedSecretEncoded { get; set; }
        public string SoftwareRevision { get; set; }
        public string SoftwareVersion { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}