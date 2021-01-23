using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Weather
{
    public class WeatherClientException : Exception
    {
        public WeatherClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
