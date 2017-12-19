using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class Message
    {
        public string Id { internal get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("priority")]
        public string Priority { get; set; }
        [JsonProperty("reliability")]
        public string Reliability { get; set; }
        [JsonProperty("eventTime")]
        public long EventTime { get; set; }
        [JsonProperty("sender")]
        public string Sender { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        public string Direction { internal get; set; }
        public double ReceivedTime { internal get; set; }
        public double SentTime { internal get; set; }
        [JsonProperty("payload")]
        public CoffeePayload Payload { get; set; }
    }
}
