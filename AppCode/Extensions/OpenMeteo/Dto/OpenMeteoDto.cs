using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo
{
  internal class OpenMeteoDto
  {
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("current")]
    public SingleResult Current { get; set; }

    [JsonPropertyName("hourly")]
    public MultipleResults Hourly { get; set; }

    public string Json { get; set; }

    public object ToCurrentModel() => new
    {
      When = Current?.Time,
      Current?.Temperature,
      Current?.WindSpeed,
      Weather = OpenMeteoConstants.GetDescription(Current?.WeatherCode ?? 0),
      Current?.WeatherCode,
      Timezone,
      Latitude,
      Longitude,
      Json
    };

    public IEnumerable<object> ToForecastModels() => (Hourly?.Time == null)
      ? Array.Empty<object>() //  new object[0]
      : Hourly.Time.Select((time, index) => new
      {
        When = time,
        Temperature = Hourly.Temperature?[index],
        WindSpeed = Hourly.WindSpeed?[index],
        WeatherCode = Hourly.WeatherCode?[index],
        Timezone,
        Latitude,
        Longitude,
        Json
      }).ToArray();
  }
}
