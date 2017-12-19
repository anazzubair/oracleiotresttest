using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class CoffeePayload
    {
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("severity")]
        public string Severity { internal get; set; }
        [JsonProperty("message")]
        public string Message { internal get; set; }
        [JsonProperty("data")]
        public CoffeeData Data { get; set; }
    }
}
