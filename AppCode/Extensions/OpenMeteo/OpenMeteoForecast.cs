using System;
using System.Linq;
using AppCode.Extensions.OpenMeteo.Data;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// DataSource which loads the hourly weather forecast from the Open-Meteo API.
  /// <br/>
  /// Returns a list of forecast records (one per hour) including temperature,
  /// wind speed, weather code and timestamp for the configured location.
  /// <br/>
  /// Intended for use in Visual Queries or directly in Razor code.
  /// </summary>
  [VisualQuery(
    NiceName = "OpenMeteo Weather Forecast",
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
    /// <summary>
    /// Fetches the hourly forecast data from Open-Meteo
    /// and returns it as a list 
    /// </summary>
    private object GetForecast()
    {
      const string fields = "temperature_2m,wind_speed_10m,weather_code";

      var result = OpenMeteoHelpers.Download(
        Kit,
        Latitude,
        Longitude,
        Timezone,
        $"&forecast_days={ForecastDays}&hourly={Uri.EscapeDataString(fields)}"
      );

      var times = result.Hourly?.Time ?? Array.Empty<string>();

      return Enumerable.Range(0, times.Length)
        .Select(i => new
        {
          Id = i + 1,
          Time = times[i],
          Temperature = result.Hourly?.Temperature?.ElementAtOrDefault(i),
          WindSpeed = result.Hourly?.WindSpeed?.ElementAtOrDefault(i),
          WeatherCode = result.Hourly?.WeatherCode?.ElementAtOrDefault(i),
          result.Timezone,
          result.Latitude,
          result.Longitude
        });
    }

    [Configuration(Fallback = "47.1674")]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration(Fallback = "9.4779")]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration(Fallback = "auto")]
    public string Timezone => Configuration.GetThis();

    [Configuration(Fallback = "2")]
    public int ForecastDays => Configuration.GetThis(2);
  }
}
