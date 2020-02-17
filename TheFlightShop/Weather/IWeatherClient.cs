using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Weather
{
    public interface IWeatherClient
    {
        Task<Weather> GetWeather();
    }
}
