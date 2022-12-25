using System.Collections.Generic;

public sealed class Settings
{
    public bool RefreshOnChange { get; set; }
    public WeatherConf Weather { get; set; }
    public Dictionary<string, Camera> Cameras { get; set; }
}