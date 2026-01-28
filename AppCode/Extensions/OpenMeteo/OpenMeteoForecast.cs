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
      var result = OpenMeteoHelpers.Download(Kit, Latitude, Longitude, Timezone,
        $"&hourly={OpenMeteoConstants.ExpectedFields}" +
        $"&forecast_days={ForecastDays}"
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

      return count == 0
        ? Array.Empty<object>()
        : result.ToForecastModels();
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
