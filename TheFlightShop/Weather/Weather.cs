using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Weather
{
    public class Weather
    {
        public WeatherMeasurement Barometer { get; set; }
        public WeatherMeasurement Temperature { get; set; }
        public WeatherMeasurement WindSpeed { get; set; }
        public WeatherMeasurement WindDirection { get; set; }
        public WeatherMeasurement Visibility { get; set; }
        public DateTime AsOf { get; set; }

        private int _expiresInMinutes;

        public Weather(DarkSkyCurrentWeather currentWeather, int expiresInMinutes)
        {
            Barometer = new WeatherMeasurement
            {
                Value = currentWeather.Pressure.ToString(), // todo
                Units = "mb"
            };
            Temperature = new WeatherMeasurement
            {
                Value = Math.Round(currentWeather.Temperature, 1).ToString(),
                Units = "F"
            };
            WindSpeed = new WeatherMeasurement
            {
                Value = currentWeather.WindSpeed.ToString(),
                Units = "mph"
            };
            WindDirection = new WeatherMeasurement
            {
                Value = currentWeather.WindBearing.ToString(),
                Units = string.Empty
            };
            Visibility = new WeatherMeasurement
            {
                Value = currentWeather.Visibility.ToString(),
                Units = "mi"
            };

            AsOf = DateTime.UtcNow;
            _expiresInMinutes = expiresInMinutes;
        }

        public bool Expired => AsOf.AddMinutes(_expiresInMinutes) < DateTime.UtcNow;
    }
}
