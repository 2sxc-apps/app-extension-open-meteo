using System;
using System.Net;
using AppCode.Extensions.OpenMeteo.Data;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// DataSource which loads the current weather conditions from the Open-Meteo API.
  /// <br/>
  /// Returns a single record containing the current temperature, wind speed,
  /// weather code and timestamp for the configured location.
  /// <br/>
  /// Intended for use in Visual Queries or directly in Razor code.
  /// </summary>
  [VisualQuery(
    NiceName = "OpenMeteo Current Weather",
    UiHint = "Loads current weather data from Open-Meteo",
    Icon = "wb_sunny",
    HelpLink = "https://open-meteo.com",
    ConfigurationType = nameof(OpenMeteoConfiguration)
  )]
  public class OpenMeteoCurrent : DataSource16
  {
    public OpenMeteoCurrent(Dependencies services) : base(services)
    {
      ProvideOut(GetCurrent);
    }
    /// <summary>
    /// Fetches the current weather data from Open-Meteo
    /// and returns it as a single record.
    /// </summary>
    private object GetCurrent()
    {
      const string fields = "temperature_2m,wind_speed_10m,weather_code";

      var result = OpenMeteoHelpers.Download(
        Kit,
        Latitude,
        Longitude,
        Timezone,
        $"&current={Uri.EscapeDataString(fields)}"
      );

      return new
      {
        result.Current.Time,
        result.Current.Temperature,
        result.Current.WindSpeed,
        result.Current.WeatherCode,
        result.Timezone,
        result.Latitude,
        result.Longitude
      };
    }

    [Configuration()]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration()]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration()]
    public string Timezone => Configuration.GetThis("auto");
  }
}
