using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers;

internal class QuerySrv : ControllerBase
{
    public static QuerySrv instance { get; set; }

    public static QuerySrv Instance
    {
        get
        {
            if (instance == null) { instance = new QuerySrv(); }
            return instance;
        }
    }

    public async Task<T> GetRequest<T>(string apiURL, string apiEntry, string authHeaderVal = null) where T : new()
    {
        var client = new HttpClient { BaseAddress = new Uri(apiURL) };
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        if (authHeaderVal != null)
        {
            client.DefaultRequestHeaders.Add("Authorization", authHeaderVal);
        }



        HttpResponseMessage response = await client.GetAsync(apiEntry);
        string data = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<T>(data) ?? new T();
        else
            return new T();
    }
}
