using JWT;
using JWT.Serializers;
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
    class Activation
    {
        AppSettingsReader appSettingsReader = new AppSettingsReader();
        BearerAccessToken bearerToken = null;
        
        public ActivationPolicy GetActivationPolicy(long deviceId)
        {
            ActivationPolicy activationPolicy = null;
            bearerToken = new Authentication().GetBearerToken(Scope.Activation, deviceId);
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("actPolicyEndpoint", typeof(string)).ToString();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.AccessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-ActivationId", appSettingsReader.GetValue("deviceActivationId", typeof(string)).ToString());

                Console.WriteLine(url);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                IJsonSerializer serializer = new JsonNetSerializer();

                using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                {
                    if (responseStream == null) return null;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseContent = streamReader.ReadToEnd();
                        Console.WriteLine(responseContent);
                        activationPolicy = serializer.Deserialize<ActivationPolicy>(responseContent);
                    }
                }
            }
            return activationPolicy;
        }

        public Boolean ActivateDevice(long deviceId)
        {
            ActivationResponse activationResponse = null;
            var activationPolicy = GetActivationPolicy(deviceId);
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("actEndpoint", typeof(string)).ToString();

            var data = new ActivationPayload().PreparePayload(deviceId);

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.AccessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("X-ActivationId", appSettingsReader.GetValue("deviceActivationId", typeof(string)).ToString());

                Console.WriteLine(url);
                HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                IJsonSerializer serializer = new JsonNetSerializer();

                using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                {
                    if (responseStream == null) return false;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var responseContent = streamReader.ReadToEnd();
                        Console.WriteLine(responseContent);
                        activationResponse = serializer.Deserialize<ActivationResponse>(responseContent);
                    }
                }
            }
            return activationResponse.EndpointState == "ACTIVATED";
        }
    }
}
