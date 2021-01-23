using FluentScheduler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace TheFlightShop.Weather
{
    public class DarkSkyWeatherClient : IWeatherClient
    {
        private Weather _currentWeather;

        private readonly int _featchIntervalMinutes;
        private readonly string _currentWeatherUrl;
        private readonly ILogger _logger;

        public DarkSkyWeatherClient(string apiToken, string latitude, string longitude, ILogger logger, int fetchIntervalMinutes = 5)
        {
            _featchIntervalMinutes = fetchIntervalMinutes;
            _currentWeatherUrl = "https://api.darksky.net/forecast/" + apiToken + "/" + latitude + "," + longitude;
            _logger = logger;
        }

        public async Task<Weather> GetWeather()
        {
            return await Task.Run(() =>
            {
                try
                {
                    lock (_currentWeather ?? new object())
                    {
                        if (_currentWeather == null || _currentWeather.Expired)
                        {
                            _currentWeather = FetchWeather();
                        }
                    }
                    return _currentWeather;
                }
                catch (Exception ex)
                {
                    throw new WeatherClientException($"{nameof(DarkSkyWeatherClient)}.{nameof(GetWeather)} - Error fetching current weather information.", ex);
                }
            });
        }

        private Weather FetchWeather()
        {            
            var weatherRequest = WebRequest.Create(_currentWeatherUrl);
            Weather currentWeather;

            using (var response = weatherRequest.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                var weather = JsonConvert.DeserializeObject<DarkSkyWeather>(content);
                currentWeather = new Weather(weather.Currently, _featchIntervalMinutes);
            }

            return currentWeather;
        }
    }
}
