using FluentScheduler;
using Microsoft.AspNetCore.Http;
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

        public DarkSkyWeatherClient(string apiToken, string latitude, string longitude, int fetchIntervalMinutes = 5)
        {
            _featchIntervalMinutes = fetchIntervalMinutes;
            _currentWeatherUrl = "https://api.darksky.net/forecast/" + apiToken + "/" + latitude + "," + longitude;
        }

        public async Task<Weather> GetWeather()
        {
            return await Task.Run(() =>
            {
                lock (_currentWeather ?? new object())
                {
                    if (_currentWeather == null || _currentWeather.Expired)
                    {
                        var weather = FetchWeather();
                        _currentWeather = weather;
                    }
                }
                return _currentWeather;
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
