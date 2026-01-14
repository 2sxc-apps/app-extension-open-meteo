using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using Custom.DataSource;

public class OpenMeteoForecast : DataSource16
{
  public OpenMeteoForecast(Dependencies services) : base(services)
  {
    ProvideOut(GetForecast);
  }

  private object GetForecast()
    {
    var latitude  = Configuration.Get<double>("Latitude", fallback: 47.1674);
    var longitude = Configuration.Get<double>("Longitude", fallback: 9.4779);
    var timezone     = Configuration.Get<string>("Timezone", fallback: "auto");
    var forecastDays = Configuration.Get<int>("ForecastDays", fallback: 2);
    var hourly       = Configuration.Get<string>("Hourly", fallback: "temperature_2m,weather_code,wind_speed_10m");

    var url =
      "https://api.open-meteo.com/v1/forecast" +
      $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
      $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
      $"&timezone={Uri.EscapeDataString(timezone)}" +
      $"&forecast_days={forecastDays}" +
      $"&hourly={Uri.EscapeDataString(hourly)}";

    var json = Download(url);

    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;
    var hourlyObj = root.GetProperty("hourly");

    var times = hourlyObj.GetProperty("time")
      .EnumerateArray()
      .Select(x => x.GetString())
      .ToList();

    var temp2m  = GetArrayOptional(hourlyObj, "temperature_2m");
    var wind10m = GetArrayOptional(hourlyObj, "wind_speed_10m");
    var code    = GetArrayOptional(hourlyObj, "weather_code");

    var items = Enumerable.Range(0, times.Count).Select(i => new
    {
      Id = i + 1,
      Title = "Open-Meteo Forecast",
      Time = times[i],

      Temperature2m = GetValue(temp2m, i),
      WindSpeed10m  = GetValue(wind10m, i),
      WeatherCode   = GetValue(code, i),

      Latitude = latitude,
      Longitude = longitude,
      Timezone = root.TryGetProperty("timezone", out var tz) ? tz.GetString() : timezone
    });

    return items.ToArray();
  }

  private static string Download(string url)
  {
    using var wc = new WebClient();
    wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
    return wc.DownloadString(url);
  }

  private static double?[] GetArrayOptional(JsonElement obj, string name)
  {
    if (!obj.TryGetProperty(name, out var arr)) return null;
    return arr.EnumerateArray()
      .Select(v => v.ValueKind == JsonValueKind.Number ? v.GetDouble() : (double?)null)
      .ToArray();
  }

  private static double? GetValue(double?[] array, int index)
  {
    return array != null && index >= 0 && index < array.Length ? array[index] : null;
  }
}
