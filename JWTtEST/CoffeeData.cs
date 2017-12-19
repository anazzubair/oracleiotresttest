using Newtonsoft.Json;

namespace JWTtEST
{
    public class CoffeeData
    {
        [JsonProperty("No_of_cups_brewed")]
        public int NumberOfCups { get; set; }
        [JsonProperty("ora_longitude")]
        public double Longitude { get; set; }
        [JsonProperty("ora_latitude")]
        public double Latitude { get; set; }
        [JsonProperty("Water_Level")]
        public double WaterLevel { get; set; }
        [JsonProperty("Beans_Level")]
        public double BeansLevel { get; set; }
    }
}