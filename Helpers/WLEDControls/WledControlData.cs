using Newtonsoft.Json;
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

        [JsonProperty("seg")]
        public List<WLEDSegment> Segment { get; set; }

        public WledControlData(string effect, int speed, int intensity, int brightness, int palette = 0)
        {
            Segment = new List<WLEDSegment>
            {
                new WLEDSegment
                {
                    Effect = WledEffectLibrary.GetWLEDEffect(effect),
                    Speed = speed,
                    Intensity = intensity,
                    ColorPalette = palette
                }
            };
            Brightness = brightness;
        }

        public async Task TurnOnKitchenWledLight()
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = this;
                controlData.On = true;
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

        public async Task TurnOnMasterBedroomNightLight()
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = this;
                controlData.On = true;
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

        public async Task TurnOnNurseryNightLight()
        {
            using (var httpClient = new HttpClient())
            {
                var controlData = this;
                controlData.On = true;
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
