using System.Collections.Generic;

namespace BasicIpCamera.Model
{
    public class WeatherConf
    {
        public int DelayBeforeStart { get; set; }
        public int Interval { get; set; }
        public Dictionary<string, StationData> Stations { get; set; }
    }
}