using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo
{
  internal class OpenMeteoResponse
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Timezone { get; set; }

    public OpenMeteoCurrentData Current { get; set; }
    public OpenMeteoHourlyData Hourly { get; set; }

    /// <summary>
    /// Raw JSON response for debugging or for further extracting additional properties returned by the API
    /// </summary>
    public string Json { get; set; }
  }

  internal class OpenMeteoCurrentData
  {
    public string Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double? Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int? WeatherCode { get; set; }
  }

  internal class OpenMeteoHourlyData
  {
    public string[] Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double?[] Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double?[] WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int?[] WeatherCode { get; set; }
  }
}
