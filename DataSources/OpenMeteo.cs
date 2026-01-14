using System;
using Custom.DataSource;

public class OpenMeteo : DataSource16
{
  public OpenMeteo(Dependencies services) : base(services)
  {
    ProvideOut(() => new[]
    {
      new { Id = 1, Title = "Hello Open-Meteo", Temperatur = 20 }
    });
  }
}
