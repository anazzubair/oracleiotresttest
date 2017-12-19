using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class ActivationPolicy
    {
        [JsonProperty("keyType")]
        public string KeyType { get; set; }
        [JsonProperty("hashAlgorithm")]
        public string HashAlgorithm { get; set; }
        [JsonProperty("keySize")]
        public string KeySize { get; set; }
    }
}
