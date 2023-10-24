using Newtonsoft.Json;

namespace ChandlerHome.Models.ResponseModels;

internal class PublicHolidayAPIResponse
{
    public DateTime date { get; set; }
    public string localName { get; set; }
    public string name { get; set; }
    public string countryCode { get; set; }
    [JsonProperty("fixed")]
    public bool fixedValue { get; set; }
    public bool global { get; set; }
    public List<string>? counties { get; set; }
    public int? launchYear { get; set; }
    public List<string>? types { get; set; }
}

public class LongWeekendAPIResponse
{
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public int dayCount { get; set; }
    public bool needBridgeDay { get; set; }
}
