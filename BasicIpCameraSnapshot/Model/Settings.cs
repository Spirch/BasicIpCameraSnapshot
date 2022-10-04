using System.Collections.Generic;

namespace BasicIpCamera.Model
{
    public class Settings
    {
        public int CacheTime { get; set; }
        public WeatherConf Weather { get; set; }
        public Dictionary<string, Camera> Cameras { get; set; }
    }
}