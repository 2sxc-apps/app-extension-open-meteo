using System;
using System.Collections.Generic;
using System.Linq;
using AppCode.Extensions.OpenMeteo.Data;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// DataSource which loads an hourly forecast from the Open-Meteo API.
  /// <br/>
  /// Returns one record per hour containing time, temperature, wind speed and weather code
  /// for the configured location.
  /// <br/>
  /// Uses the strongly typed OpenMeteoResult model.
  /// </summary>
  [VisualQuery(
    NiceName = "OpenMeteo Forecast",
    UiHint = "Loads hourly forecast data from Open-Meteo",
    Icon = "schedule",
    HelpLink = "https://open-meteo.com",
    ConfigurationType = nameof(OpenMeteoConfiguration)
  )]
  public class OpenMeteoForecast : DataSource16
  {
    public OpenMeteoForecast(Dependencies services) : base(services)
    {
      ProvideOut(GetForecast);
    }

    private IEnumerable<object> GetForecast()
    {
      // Keep hourly fields as an implementation detail of the DataSource
      const string hourlyFields = "temperature_2m,wind_speed_10m,weather_code";

      var extras =
        $"&hourly={Uri.EscapeDataString(hourlyFields)}" +
        $"&forecast_days={ForecastDays}";

      var result = OpenMeteoHelpers.Download(
        Kit,
        Latitude,
        Longitude,
        Timezone,
        extras
      );

      var hourly = result.Hourly;


      // Ensure we don't index out of range if some arrays are missing/shorter
      var count = new[]
      {
        hourly.Time?.Length ?? 0,
        hourly.Temperature?.Length ?? 0,
        hourly.WindSpeed?.Length ?? 0,
        hourly.WeatherCode?.Length ?? 0
      }.Where(l => l > 0).DefaultIfEmpty(0).Min();

      if (count == 0)
        return Array.Empty<object>();

      return Enumerable.Range(0, count).Select(index =>
      {
        var code = hourly.WeatherCode?[index] ?? 0;

        return new
        {
          // keep property names aligned with your Razor sample
          Time = hourly.Time[index],
          Temperature = hourly.Temperature?[index],
          WindSpeed = hourly.WindSpeed?[index],
          WeatherCode = hourly.WeatherCode?[index],

          Weather = OpenMeteoConstants.WeatherInterpretationCodes.TryGetValue(code, out var desc) ? desc : "Unknown",
          result.Timezone,
          result.Latitude,
          result.Longitude,
          result.Json
        };
      });
    }

    [Configuration()]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration()]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration()]
    public string Timezone => Configuration.GetThis("auto");

    [Configuration()]
    public int ForecastDays => Configuration.GetThis(2);
  }
}
