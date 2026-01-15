using System;
using System.Net;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace AppCode.Extensions.OpenMeteo
{
    [VisualQuery(
    NiceName = "OpenMeteo Current Weather",
    UiHint = "Loads current weather data from Open-Meteo",
    Icon = "wb_sunny",
    HelpLink = "https://open-meteo.com",
    ConfigurationType = "OpenMeteoConfiguration"
  )]
  public class OpenMeteoCurrent : DataSource16
  {
    public OpenMeteoCurrent(Dependencies services) : base(services)
    {
      ProvideOut(GetCurrent);
    }

    private object GetCurrent()
    {
      const string CurrentFields = "temperature_2m,wind_speed_10m,weather_code";

      var url =
        "https://api.open-meteo.com/v1/forecast" +
        $"?latitude={Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&longitude={Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&timezone={Uri.EscapeDataString(Timezone)}" +
        $"&current={Uri.EscapeDataString(CurrentFields)}";

      var json = Download(url);
      var result = Kit.Convert.Json.To<OpenMeteoResponse>(json);

      return new
      {
        result.Current.Time,
        result.Current.Temperature2m,
        result.Current.WindSpeed10m,
        result.Current.WeatherCode,
        result.Timezone,
        result.Latitude,
        result.Longitude
      };
    }

    [Configuration(Fallback = "47.1674")]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration(Fallback = "9.4779")]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration(Fallback = "auto")]
    public string Timezone => Configuration.GetThis();

    private static string Download(string url)
    {
      using var wc = new WebClient();
      wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
      return wc.DownloadString(url);
    }
  }
}
