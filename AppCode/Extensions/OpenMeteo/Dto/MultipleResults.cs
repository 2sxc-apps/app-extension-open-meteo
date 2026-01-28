using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo
{
  internal class MultipleResults
  {
    [JsonPropertyName("time")]
    public string[] Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double?[] Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double?[] WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int?[] WeatherCode { get; set; }
  }
}
