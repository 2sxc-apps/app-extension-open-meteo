using System;
using System.Net;
using Custom.DataSource;

namespace AppCode.Extensions.DataSourceOpenMeteo
{
  public class OpenMeteoCurrent : DataSource16
  {
    public OpenMeteoCurrent(Dependencies services) : base(services)
    {
      ProvideOut(GetCurrent);
    }

    private object GetCurrent()
    {
      var latitude  = Configuration.Get<double>("Latitude", fallback: 47.1674);
      var longitude = Configuration.Get<double>("Longitude", fallback: 9.4779);
      var timezone  = Configuration.Get<string>("Timezone", fallback: "auto");

      const string CurrentFields = "temperature_2m,wind_speed_10m,weather_code";

      var url =
        "https://api.open-meteo.com/v1/forecast" +
        $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&timezone={Uri.EscapeDataString(timezone)}" +
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

    private static string Download(string url)
    {
      using var wc = new WebClient();
      wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
      return wc.DownloadString(url);
    }
  }
}
