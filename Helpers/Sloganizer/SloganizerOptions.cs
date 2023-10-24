using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.Sloganizer
{
    internal class SloganizerOptions : ISloganizerOptions
    {
        public HttpClient _httpClient { get; set; }
        public SloganizerOptions(HttpClient client, string baseUrl)
        {
            _httpClient = client;
            BaseUrl = baseUrl;
        }
        public string BaseUrl { get; set; }
    }
}
