using System.Collections.Generic;

namespace WeatherUpdate.Model;

public sealed class Settings
{
    public bool RefreshOnChange { get; set; }
    public WeatherConf Weather { get; set; }
    public Dictionary<string, Camera> Cameras { get; set; }
}