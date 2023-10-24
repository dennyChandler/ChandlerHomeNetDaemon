using Newtonsoft.Json;

namespace ChandlerHome.Helpers.WLEDControls
{
    internal class WLEDSegment
    {
        [JsonProperty("fx")]
        public int Effect { get; set; }
        [JsonProperty("sx")]
        public int Speed { get; set; }
        [JsonProperty("ix")]
        public int Intensity { get; set; }
        [JsonProperty("pal")]
        public int ColorPalette { get; set; }
    }
}
