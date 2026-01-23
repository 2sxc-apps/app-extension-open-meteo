using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo
{
  public class OpenMeteoResponse
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Timezone { get; set; }

    public OpenMeteoCurrentData Current { get; set; }
    public OpenMeteoHourlyData Hourly { get; set; }
  }

  public class OpenMeteoCurrentData
  {
    public string Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double? Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int? WeatherCode { get; set; }
  }

  public class OpenMeteoHourlyData
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
