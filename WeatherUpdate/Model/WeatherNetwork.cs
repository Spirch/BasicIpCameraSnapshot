using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WeatherUpdate.Model.ECCC;

namespace WeatherUpdate.Model.WeatherNetwork
{
    // WeatherNetwork myDeserializedClass = JsonConvert.DeserializeObject<SiteData>(myJsonResponse);
    internal sealed class SiteData : ISiteData
    {
        public Observation observation { get; set; }
        public Display display { get; set; }
        public long LastUpdate => long.Parse(System.DateTime.Parse(observation.time.local).ToString("yyyyMMddHHmm"));

        public override string ToString()
        {
            return $"{observation.temperature}{display.unit.temperature}({observation.feelsLike}) {observation.relativeHumidity}{display.unit.relativeHumidity} {observation.wind.speed}({observation.wind.gust}){observation.wind.direction} {observation.weatherCode.overlay} ";
        }
    }

    public sealed class Display
    {
        public string imageUrl { get; set; }
        public Unit unit { get; set; }
    }

    public sealed class Observation
    {
        public Time time { get; set; }
        public WeatherCode weatherCode { get; set; }
        public int temperature { get; set; }
        public int dewPoint { get; set; }
        public int feelsLike { get; set; }
        public Wind wind { get; set; }
        public int relativeHumidity { get; set; }
        public Pressure pressure { get; set; }
        public double visibility { get; set; }
        public int ceiling { get; set; }
    }

    public sealed class Pressure
    {
        public double value { get; set; }
        public int trendKey { get; set; }
    }

    public sealed class Time
    {
        public string local { get; set; }
        public string utc { get; set; }
    }

    public sealed class Unit
    {
        public string temperature { get; set; }
        public string dewPoint { get; set; }
        public string wind { get; set; }
        public string relativeHumidity { get; set; }
        public string pressure { get; set; }
        public string visibility { get; set; }
        public string ceiling { get; set; }
    }

    public sealed class WeatherCode
    {
        public string value { get; set; }
        public int icon { get; set; }
        public string text { get; set; }
        public string bgimage { get; set; }
        public string overlay { get; set; }
    }

    public sealed class Wind
    {
        public string direction { get; set; }
        public int speed { get; set; }
        public int gust { get; set; }
    }
}
