using System;
using System.Net;
using System.Text.Json;
using Custom.DataSource;

public class OpenMeteoCurrent : DataSource16
{
  public OpenMeteoCurrent(Dependencies services) : base(services)
  {
    ProvideOut(GetCurrentData);
  }

  private object GetCurrentData()
  {
    var latitude  = Configuration.Get<double>("Latitude", fallback: 47.1674);
    var longitude = Configuration.Get<double>("Longitude", fallback: 9.4779);
    var timezone  = Configuration.Get<string>("Timezone", fallback: "auto");
    var current   = Configuration.Get<string>("Current", fallback: "temperature_2m,wind_speed_10m,weather_code");

    var url =
      "https://api.open-meteo.com/v1/forecast" +
      $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
      $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
      $"&timezone={Uri.EscapeDataString(timezone)}" +
      $"&current={Uri.EscapeDataString(current)}";

    var json = Download(url);

    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;
    var currentObj = root.GetProperty("current");

    var item = new
    {
      Id = 1,
      Title = "Open-Meteo Current",
      Latitude = latitude,
      Longitude = longitude,
      Timezone = root.GetProperty("timezone").GetString(),
      Time = currentObj.GetProperty("time").GetString(),

      Temperature2m = GetNumber(currentObj, "temperature_2m"),
      WindSpeed10m  = GetNumber(currentObj, "wind_speed_10m"),
      WeatherCode  = GetNumber(currentObj, "weather_code"),
    };

    return new[] { item };
  }

  private static string Download(string url)
  {
    using var wc = new WebClient();
    wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");
    return wc.DownloadString(url);
  }

  private static double? GetNumber(JsonElement obj, string name)
  {
    return obj.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.Number
      ? el.GetDouble()
      : (double?)null;
  }
}
