using JWT;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class MessageSender
    {
        public bool Send(long deviceId, long count=1)
        {
            Message message = null;
            var deviceEndpointId = String.Empty;
            using (var db = new IoTContext())
            {
                var device = db.Devices.Find(deviceId);
                deviceEndpointId = device.DeviceEndpointId;
                message = new Message
                {
                    ClientId = Guid.NewGuid().ToString(),
                    Source = device.DeviceEndpointId,
                    Destination = "",
                    Priority = "LOW",
                    Reliability = "BEST_EFFORT",
                    EventTime = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                    Sender = "",
                    Type = "DATA",
                    Payload = new CoffeePayload
                    {
                        Format = "urn:com:keurig:coffee:machine:attributes",
                        Data = new CoffeeData
                        {
                            BeansLevel = 10,
                            WaterLevel = 80,
                            Latitude = 37.39,
                            Longitude = -121.95
                        }
                    }
                };
            }
            var messages = new List<Message> { message };
            var data = JsonConvert.SerializeObject(messages, Newtonsoft.Json.Formatting.Indented);

            Console.WriteLine("Message: " +  data);

            var bearerToken = new Authentication().GetBearerToken(Scope.General, deviceId);

            var appSettingsReader = new AppSettingsReader();
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("messEndpoint", typeof(string)).ToString();
            //Message messageResponse = null;

            for (var i = 0; i < count; i++)
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/json");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.AccessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("X-EndpointId", deviceEndpointId);

                    Console.WriteLine(url);
                    HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                    IJsonSerializer serializer = new JsonNetSerializer();
                    Console.WriteLine($"Response Code: {response.StatusCode}");
                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        if (responseStream == null) return false;
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var responseContent = streamReader.ReadToEnd();
                            Console.WriteLine(responseContent);
                            //messageResponse = serializer.Deserialize<Message>(responseContent);
                        }
                    }

                }
            }
            return true;
        }
    }
}
