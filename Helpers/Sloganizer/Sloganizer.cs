﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.Sloganizer
{
    public class Sloganizer
    {
        string baseUrl;
        public Sloganizer(ISloganizerOptions options)
        {
            baseUrl = options.BaseUrl;
            
        }
        public async Task<string> GetSlogan(string sloganWord)
        {
            var _httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{baseUrl}/en/outbound.php?slogan={sloganWord}"));
            request.Headers.Accept.Clear();

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var message = body.Replace(@"<a href='http://www.sloganizer.net/en/' title='Generated by Sloganizer.net' style='text-decoration:none;'>", "");
            message = message.Replace(@"</a>", "");
            return message;
        }
    }
}