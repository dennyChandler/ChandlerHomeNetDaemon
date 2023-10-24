using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.WLEDControls
{
    internal class WledControlData
    {

        private const string KitchenWledApiUrl = "http://192.168.1.126/json/state";
        private const string BedroomWledApiUrl = "http://192.168.1.209/json/state";
        private const string NurseryWledApiUrl = "http://192.168.1.10/json/state";

        [JsonProperty("on")]
        public bool On { get; set; }

        [JsonProperty("bri")]
        public int Brightness { get; set; }

        [JsonProperty("fx")]
        public int Effect { get; set; }

        public async Task TurnOnKitchenWledLight(string effect, int brightness)
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = new WledControlData();
                var jsonData = JsonConvert.SerializeObject(controlData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request to set the effect and brightness
                var response = await httpClient.PostAsync(KitchenWledApiUrl, content);

                // Check if the request was successful (HTTP status code 200)
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }

        public async Task TurnOnMasterBedroomNightLight(string effect, int brightness)
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = new WledControlData { Brightness = brightness, On = true, Effect = WledEffectLibrary.GetWLEDEffect(effect) };
                var jsonData = JsonConvert.SerializeObject(controlData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request to set the effect and brightness
                var response = await httpClient.PostAsync(BedroomWledApiUrl, content);

                // Check if the request was successful (HTTP status code 200)
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }

        public async Task TurnOnNurseryNightLight(string effect, int brightness)
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = new WledControlData { Brightness = brightness, On = true, Effect = WledEffectLibrary.GetWLEDEffect(effect) };
                var jsonData = JsonConvert.SerializeObject(controlData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request to set the effect and brightness
                var response = await httpClient.PostAsync(NurseryWledApiUrl, content);

                // Check if the request was successful (HTTP status code 200)
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
