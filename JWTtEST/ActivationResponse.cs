using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class ActivationResponse
    {
        [JsonProperty("endpointId")]
        public string EndpointID { get; set; }
        [JsonProperty("activationTime")]
        public string ActivationTime { get; set; }
        [JsonProperty("endpointState")]
        public string EndpointState { get; set; }
    }
}
