using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    public enum Scope
    {
        General, Activation
    }

    class Authentication
    {
        AppSettingsReader appSettingsReader = new AppSettingsReader();
        public BearerAccessToken GetBearerToken(Scope scope, long deviceId)
        {
            BearerAccessToken bearerToken = null;
            string jwtToken = GetJwtToken(scope, deviceId);
            Console.WriteLine($"JWT Token: {jwtToken}");

            var username = appSettingsReader.GetValue("username", typeof(string)).ToString();
            var password = appSettingsReader.GetValue("password", typeof(string)).ToString();
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("authEndpoint", typeof(string)).ToString();

            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "client_assertion", jwtToken },
                { "scope", scope == Scope.Activation ? "oracle/iot/activation" : "" }
            }.AsEnumerable();

            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(data);
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                String authHeader = System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeader);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                IJsonSerializer serializer = new JsonNetSerializer();

                using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                {
                    if (responseStream == null) return null;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseContent = streamReader.ReadToEnd();
                        Console.WriteLine(responseContent);
                        bearerToken = serializer.Deserialize<BearerAccessToken>(responseContent);
                        Console.WriteLine($"Bearer {bearerToken.AccessToken}");
                    }
                }
            }

            return bearerToken;
        }

        private string GetJwtToken(Scope scope, long deviceId)
        {
            var activationSecret = String.Empty;
            var privateKey = String.Empty;
            var deviceEndpointId = String.Empty;
            var activationId = String.Empty;
            var deviceRSAXml = String.Empty;

            using (var db = new IoTContext())
            {
                var device = db.Devices.Where(d => d.Id == 1).Include(d => d.Registration).First();
                activationSecret = device.Registration.SharedSecret;
                deviceEndpointId = device.Registration.DeviceEnpointId;
                activationId = device.Registration.HardwareId;
                deviceRSAXml = device.RSAKeyXML;
            }

            IDateTimeProvider provider = new UtcDateTimeProvider();
            var expiresOn = provider.GetNow().AddMinutes(20);
            //var secret = appSettingsReader.GetValue(scope == Scope.Activation ? "activationSecret" : "privateKey", typeof(string)).ToString();

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var secondsSinceEpoch = Math.Round((provider.GetNow() - unixEpoch).TotalSeconds);


            var payload = new Dictionary<string, object>
            {
                { "iss", scope == Scope.Activation ? activationId : deviceEndpointId },
                { "exp", Convert.ToInt32(secondsSinceEpoch) },
                { "aud", appSettingsReader.GetValue("jwtAudience", typeof(string)).ToString() }
            };

            if (scope == Scope.Activation)
            {
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var jwtToken = encoder.Encode(payload, activationSecret);
                return jwtToken;
            }
            else
            {
                RSACryptoServiceProvider deviceRSA = new RSACryptoServiceProvider();
                deviceRSA.FromXmlString(deviceRSAXml);
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

                var segments = new List<string>(3);

                var header = new Dictionary<string, object>();
                header.Add("typ", "JWT");
                header.Add("alg", "RS256");

                var headerBytes = Encoding.UTF8.GetBytes(serializer.Serialize(header));
                var payloadBytes = Encoding.UTF8.GetBytes(serializer.Serialize(payload));

                segments.Add(urlEncoder.Encode(headerBytes));
                segments.Add(urlEncoder.Encode(payloadBytes));

                var stringToSign = String.Join(".", segments.ToArray());
                var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

                var signature = deviceRSA.SignData(bytesToSign, new SHA256Managed());
                segments.Add(urlEncoder.Encode(signature));

                return String.Join(".", segments.ToArray());
            }
        }
    }
}
