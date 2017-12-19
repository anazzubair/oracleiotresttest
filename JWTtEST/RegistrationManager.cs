using JWT;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTtEST
{
    class RegistrationManager
    {
        AppSettingsReader appSettingsReader = new AppSettingsReader();
        public Device RegisterDevice(long deviceId)
        {
            var username = appSettingsReader.GetValue("username", typeof(string)).ToString();
            var password = appSettingsReader.GetValue("password", typeof(string)).ToString();
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("regEndpoint", typeof(string)).ToString();

            JObject data;
            using (var db = new IoTContext())
            {
                var device = db.Devices.Where(d => d.Id == deviceId).Include(d => d.Registration).First();
                var registration = device.Registration;

                data = JObject.FromObject(new
                {
                    hardwareId = registration.HardwareId,
                    sharedSecret = Convert.ToBase64String(Encoding.ASCII.GetBytes(registration.SharedSecret)),
                    name = registration.Name
                });

                registration.Request = data.ToString();
                db.SaveChanges();
            }

            String responseContent;

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/json");

                String authHeader = System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer " + authHeader);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeader);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine($"Registration URL {url}");
                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                IJsonSerializer serializer = new JsonNetSerializer();
                var registration = new Registration();

                using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                {
                    if (responseStream == null) return null;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        responseContent = streamReader.ReadToEnd();
                        Console.WriteLine(responseContent);
                        registration = serializer.Deserialize<Registration>(responseContent);
                        Console.WriteLine($"Registration Status: {registration.State}");
                    }
                }
            }

            using (var db = new IoTContext())
            {
                var device = db.Devices.Where(d => d.Id == deviceId).Include(d => d.Registration).First();
                var registration = device.Registration;
                registration.Response = responseContent;

                var deviceRSA = new RSACryptoServiceProvider(2048);
                var deviceRSAXml = deviceRSA.ToXmlString(true);
                device.RSAKeyXML = deviceRSAXml;

                db.SaveChanges();
            }
            return null;
        }
    }
}
