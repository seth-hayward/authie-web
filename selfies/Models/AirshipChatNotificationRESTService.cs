using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace selfies.Models
{
    public class AirshipChatNotificationRESTService
    {

        readonly string uri = "https://go.urbanairship.com/api/push/";
        readonly string echo_uri = "https://go.urbanairship.com/api/echo/?msg=hello";

        public async Task<AirshipResponse> SendChat(string device_token, string message)
        {

            AirshipResponse arr = new AirshipResponse();

            string username = "bowqtjANSiadJX-dQQjikg";
            string password = "6pFP8TKoSEqNpRiyxgYEgA";

            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);

            var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", Convert.ToBase64String(byteArray));

            // ...
            AirshipNotification note = new AirshipNotification
            {
                aps = new AirshipNotification.AirshipBody
                {
                    Alert = message
                },
                device_tokens = new List<string>
                {
                    device_token
                }
            };

            string js = JsonConvert.SerializeObject(note);

            //HttpResponseMessage response = await client.GetAsync(echo_uri);
            HttpResponseMessage response = await client.PostAsync(uri, new StringContent(js, Encoding.UTF8, "application/json"));
            HttpContent content = response.Content;

            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

            arr.result = 1;
            arr.message = "Success";

            return arr;

        }

        private AuthenticationHeaderValue CreateBasicAuthenticationHeader(string username, string password)
        {
            return new AuthenticationHeaderValue(
            "Basic",
            System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(
            string.Format("{0}:{1}", username, password)))
            );
        }

    }
}