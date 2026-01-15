using System;
using System.Linq;
using System.Net;
using Custom.DataSource;

namespace AppCode.Extensions.DataSourceOpenMeteo
{
  public class OpenMeteoForecast : DataSource16
  {
    public OpenMeteoForecast(Dependencies services) : base(services)
    {
      ProvideOut(GetForecast);
    }

    private object GetForecast()
    {
      var latitude     = Configuration.Get<double>("Latitude", fallback: 47.1674);
      var longitude    = Configuration.Get<double>("Longitude", fallback: 9.4779);
      var timezone     = Configuration.Get<string>("Timezone", fallback: "auto");
      var forecastDays = Configuration.Get<int>("ForecastDays", fallback: 2);

      const string HourlyFields = "temperature_2m,wind_speed_10m,weather_code";

      var url =
        "https://api.open-meteo.com/v1/forecast" +
        $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
        $"&timezone={Uri.EscapeDataString(timezone)}" +
        $"&forecast_days={forecastDays}" +
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

    private static string Download(string url)
    {
      using var wc = new WebClient();
      wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
      return wc.DownloadString(url);
    }
  }
}
