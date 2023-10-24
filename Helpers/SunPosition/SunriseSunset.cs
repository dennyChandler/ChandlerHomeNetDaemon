using ChandlerHome.Helpers.Sloganizer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.SunPosition
{
    public class SunriseSunset
    {
        public DateTime? sunrise { get; set; }
        public DateTime? sunset { get; set; }
        public DateTime? solar_noon { get; set; }
        public DateTime? day_length { get; set; }
        public DateTime? civil_twilight_begin { get; set; }
        public DateTime? civil_twilight_end { get; set; }
        public DateTime? nautical_twilight_begin { get; set; }
        public DateTime? nautical_twilight_end { get; set; }
        public DateTime? astronomical_twilight_begin { get; set; }
        public DateTime? astronomical_twilight_end { get; set; }
        string baseUrl;
        private readonly ILogger<SunriseSunset> _logger;

        public SunriseSunset(ILogger<SunriseSunset> logger)
        {
            _logger = logger;
        }

        public async Task<SunriseSunsetResult> GetSunriseSunsetInfo()
        {
            var _httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.sunrise-sunset.org/json?lat=38.902210&lng=-94.390810"));
            request.Headers.Accept.Clear();

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                SunriseSunsetResult message = JsonSerializer.Deserialize<SunriseSunsetResult>(body);
                return message;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception during sunriseSunset get and parse! {e.StackTrace} \n {e.Message}");
                return null;
            }
        }
    }
}
