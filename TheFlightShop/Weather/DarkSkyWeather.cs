using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Weather
{
    public class DarkSkyWeather
    {
        [JsonProperty("currently")]
        public DarkSkyCurrentWeather Currently { get; set; }
    }
}
