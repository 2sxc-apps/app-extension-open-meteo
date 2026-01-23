using System;
using System.Net;
using ToSic.Sxc.Services;

namespace AppCode.Extensions.OpenMeteo
{
  public static class OpenMeteoHelpers
  {
    private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

    public static OpenMeteoResponse Download(
      ServiceKitLight16 kit,
      double latitude,
      double longitude,
      string timezone,
      string extraQuery
    )
    {
      var url = BaseUrl +
        $"?latitude={kit.Convert.ForCode(latitude)}" +
        $"&longitude={kit.Convert.ForCode(longitude)}" +
        $"&timezone={Uri.EscapeDataString(timezone)}" +
        extraQuery;

      using var wc = new WebClient();
      wc.Headers.Add("User-Agent", "2sxc OpenMeteo DataSource");

      var json = wc.DownloadString(url);
      return kit.Convert.Json.To<OpenMeteoResponse>(json);
    }
  }
}
