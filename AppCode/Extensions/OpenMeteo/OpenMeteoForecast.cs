using System;
using System.Linq;
using System.Net;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery; // This namespace is for the [Configuration] attribute

namespace AppCode.Extensions.OpenMeteo
{
  [VisualQuery(
    NiceName = "OpenMeteo Weather Forecast",
    UiHint = "Loads hourly forecast data from Open-Meteo",
    Icon = "schedule",
    HelpLink = "https://open-meteo.com",
    ConfigurationType = "OpenMeteoConfiguration"
  )]
  public class OpenMeteoForecast : DataSource16
  {
    public OpenMeteoForecast(Dependencies services) : base(services)
    {
      ProvideOut(GetForecast);
    }

    private object GetForecast()
    {
      const string HourlyFields = "temperature_2m,wind_speed_10m,weather_code";

      var url =
        "https://api.open-meteo.com/v1/forecast" +
        $"?latitude={Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&longitude={Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&timezone={Uri.EscapeDataString(Timezone)}" +
        $"&forecast_days={ForecastDays}" +
        $"&hourly={Uri.EscapeDataString(HourlyFields)}";

      var json = Download(url);
      var result = Kit.Convert.Json.To<OpenMeteoResponse>(json);

      var times = result.Hourly?.Time ?? Array.Empty<string>();

      return Enumerable.Range(0, times.Length).Select(i => new
      {
        Id = i + 1,
        Time = times[i],
        Temperature2m = result.Hourly?.Temperature2m != null && i < result.Hourly.Temperature2m.Length ? result.Hourly.Temperature2m[i] : null,
        WindSpeed10m  = result.Hourly?.WindSpeed10m  != null && i < result.Hourly.WindSpeed10m.Length  ? result.Hourly.WindSpeed10m[i]  : null,
        WeatherCode   = result.Hourly?.WeatherCode   != null && i < result.Hourly.WeatherCode.Length   ? result.Hourly.WeatherCode[i]   : null,
        result.Timezone,
        result.Latitude,
        result.Longitude
      });
    }

    // Configuration properties using [Configuration] attribute
    [Configuration(Fallback = "47.1674")]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration(Fallback = "9.4779")]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration(Fallback = "auto")]
    public string Timezone => Configuration.GetThis();

    [Configuration(Fallback = "2")]
    public int ForecastDays => Configuration.GetThis(2);

    private static string Download(string url)
    {
      using var wc = new WebClient();
      wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
      return wc.DownloadString(url);
    }
  }
}
