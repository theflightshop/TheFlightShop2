using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Weather
{
    public class DarkSkyCurrentWeather
    {
        [JsonProperty("temperature")]
        public decimal Temperature { get; set; }
        [JsonProperty("pressure")]
        public decimal Pressure { get; set; }
        [JsonProperty("windSpeed")]
        public decimal WindSpeed { get; set; }
        [JsonProperty("windBearing")]
        public decimal WindBearing { get; set; }
        [JsonProperty("visibility")]
        public decimal Visibility { get; set; }
    }
}
