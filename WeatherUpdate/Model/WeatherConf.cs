using System.Collections.Generic;

namespace WeatherUpdate.Model;

public sealed class WeatherConf
{
    public int DelayBeforeStart { get; set; }
    public int Interval { get; set; }
    public Dictionary<string, StationData> Stations { get; set; }
}