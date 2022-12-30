using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WeatherUpdate.Model.WeatherNetwork;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherUpdate.Model.ECCC
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(SiteData));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (SiteData)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "month")]
    public class Month
    {

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "day")]
    public class Day
    {

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "dateTime")]
    public class DateTime
    {

        [XmlElement(ElementName = "year")]
        public string Year { get; set; }

        [XmlElement(ElementName = "month")]
        public Month Month { get; set; }

        [XmlElement(ElementName = "day")]
        public Day Day { get; set; }

        [XmlElement(ElementName = "hour")]
        public string Hour { get; set; }

        [XmlElement(ElementName = "minute")]
        public string Minute { get; set; }

        [XmlElement(ElementName = "timeStamp")]
        public string TimeStamp { get; set; }

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "zone")]
        public string Zone { get; set; }

        [XmlAttribute(AttributeName = "UTCOffset")]
        public string UTCOffset { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "country")]
    public class Country
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "province")]
    public class Province
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "name")]
    public class Name
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public string Lat { get; set; }

        [XmlAttribute(AttributeName = "lon")]
        public string Lon { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "location")]
    public class Location
    {

        [XmlElement(ElementName = "continent")]
        public string Continent { get; set; }

        [XmlElement(ElementName = "country")]
        public Country Country { get; set; }

        [XmlElement(ElementName = "province")]
        public Province Province { get; set; }

        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }

        [XmlElement(ElementName = "region")]
        public string Region { get; set; }
    }

    [XmlRoot(ElementName = "event")]
    public class Event
    {

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "priority")]
        public string Priority { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "warnings")]
    public class Warnings
    {

        [XmlElement(ElementName = "event")]
        public Event Event { get; set; }

        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "station")]
    public class Station
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public string Lat { get; set; }

        [XmlAttribute(AttributeName = "lon")]
        public string Lon { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "iconCode")]
    public class IconCode
    {

        [XmlAttribute(AttributeName = "format")]
        public string Format { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "temperature")]
    public class Temperature
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }

        [XmlAttribute(AttributeName = "period")]
        public string Period { get; set; }

        [XmlAttribute(AttributeName = "year")]
        public string Year { get; set; }
    }

    [XmlRoot(ElementName = "dewpoint")]
    public class Dewpoint
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "windChill")]
    public class WindChill
    {

        [XmlElement(ElementName = "calculated")]
        public List<Calculated> Calculated { get; set; }

        [XmlElement(ElementName = "frostbite")]
        public object Frostbite { get; set; }

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "pressure")]
    public class Pressure
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlAttribute(AttributeName = "change")]
        public string Change { get; set; }

        [XmlAttribute(AttributeName = "tendency")]
        public string Tendency { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "visibility")]
    public class Visibility
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "windVisib")]
        public WindVisib WindVisib { get; set; }
    }

    [XmlRoot(ElementName = "relativeHumidity")]
    public class RelativeHumidity
    {

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "speed")]
    public class Speed
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "gust")]
    public class Gust
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "bearing")]
    public class Bearing
    {

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "wind")]
    public class Wind
    {

        [XmlElement(ElementName = "direction")]
        public Direction Direction { get; set; }

        [XmlElement(ElementName = "gust")]
        public Gust Gust { get; set; }

        [XmlElement(ElementName = "speed")]
        public Speed Speed { get; set; }
    }

    [XmlRoot(ElementName = "currentConditions")]
    public class CurrentConditions
    {

        [XmlElement(ElementName = "station")]
        public Station Station { get; set; }

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }

        [XmlElement(ElementName = "condition")]
        public string Condition { get; set; }

        [XmlElement(ElementName = "iconCode")]
        public IconCode IconCode { get; set; }

        [XmlElement(ElementName = "temperature")]
        public Temperature Temperature { get; set; }

        [XmlElement(ElementName = "dewpoint")]
        public Dewpoint Dewpoint { get; set; }

        [XmlElement(ElementName = "windChill")]
        public WindChill WindChill { get; set; }

        [XmlElement(ElementName = "pressure")]
        public Pressure Pressure { get; set; }

        [XmlElement(ElementName = "visibility")]
        public Visibility Visibility { get; set; }

        [XmlElement(ElementName = "relativeHumidity")]
        public RelativeHumidity RelativeHumidity { get; set; }

        [XmlElement(ElementName = "wind")]
        public Wind Wind { get; set; }
    }

    [XmlRoot(ElementName = "regionalNormals")]
    public class RegionalNormals
    {

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlElement(ElementName = "temperature")]
        public List<Temperature> Temperature { get; set; }
    }

    [XmlRoot(ElementName = "period")]
    public class Period
    {

        [XmlAttribute(AttributeName = "textForecastName")]
        public string TextForecastName { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "cloudPrecip")]
    public class CloudPrecip
    {

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }
    }

    [XmlRoot(ElementName = "pop")]
    public class Pop
    {

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "abbreviatedForecast")]
    public class AbbreviatedForecast
    {

        [XmlElement(ElementName = "iconCode")]
        public IconCode IconCode { get; set; }

        [XmlElement(ElementName = "pop")]
        public Pop Pop { get; set; }

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }
    }

    [XmlRoot(ElementName = "temperatures")]
    public class Temperatures
    {

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlElement(ElementName = "temperature")]
        public Temperature Temperature { get; set; }
    }

    [XmlRoot(ElementName = "winds")]
    public class Winds
    {

        [XmlElement(ElementName = "wind")]
        public List<Wind> Wind { get; set; }
    }

    [XmlRoot(ElementName = "precipType")]
    public class PrecipType
    {

        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; }

        [XmlAttribute(AttributeName = "end")]
        public string End { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "amount")]
    public class Amount
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "accumulation")]
    public class Accumulation
    {

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "amount")]
        public Amount Amount { get; set; }
    }

    [XmlRoot(ElementName = "precipitation")]
    public class Precipitation
    {

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlElement(ElementName = "precipType")]
        public PrecipType PrecipType { get; set; }

        [XmlElement(ElementName = "accumulation")]
        public Accumulation Accumulation { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }

        [XmlAttribute(AttributeName = "period")]
        public string Period { get; set; }

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlAttribute(AttributeName = "year")]
        public string Year { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "calculated")]
    public class Calculated
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "index")]
        public string Index { get; set; }
    }

    [XmlRoot(ElementName = "windVisib")]
    public class WindVisib
    {

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlAttribute(AttributeName = "cause")]
        public string Cause { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "forecast")]
    public class Forecast
    {

        [XmlElement(ElementName = "period")]
        public Period Period { get; set; }

        [XmlElement(ElementName = "textSummary")]
        public string TextSummary { get; set; }

        [XmlElement(ElementName = "cloudPrecip")]
        public CloudPrecip CloudPrecip { get; set; }

        [XmlElement(ElementName = "abbreviatedForecast")]
        public AbbreviatedForecast AbbreviatedForecast { get; set; }

        [XmlElement(ElementName = "temperatures")]
        public Temperatures Temperatures { get; set; }

        [XmlElement(ElementName = "winds")]
        public Winds Winds { get; set; }

        [XmlElement(ElementName = "humidex")]
        public object Humidex { get; set; }

        [XmlElement(ElementName = "precipitation")]
        public Precipitation Precipitation { get; set; }

        [XmlElement(ElementName = "windChill")]
        public WindChill WindChill { get; set; }

        [XmlElement(ElementName = "visibility")]
        public Visibility Visibility { get; set; }

        [XmlElement(ElementName = "relativeHumidity")]
        public RelativeHumidity RelativeHumidity { get; set; }
    }

    [XmlRoot(ElementName = "forecastGroup")]
    public class ForecastGroup
    {

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }

        [XmlElement(ElementName = "regionalNormals")]
        public RegionalNormals RegionalNormals { get; set; }

        [XmlElement(ElementName = "forecast")]
        public List<Forecast> Forecast { get; set; }
    }

    [XmlRoot(ElementName = "lop")]
    public class Lop
    {

        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "humidex")]
    public class Humidex
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }
    }

    [XmlRoot(ElementName = "direction")]
    public class Direction
    {

        [XmlAttribute(AttributeName = "windDirFull")]
        public string WindDirFull { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "hourlyForecast")]
    public class HourlyForecast
    {

        [XmlElement(ElementName = "condition")]
        public string Condition { get; set; }

        [XmlElement(ElementName = "iconCode")]
        public IconCode IconCode { get; set; }

        [XmlElement(ElementName = "temperature")]
        public Temperature Temperature { get; set; }

        [XmlElement(ElementName = "lop")]
        public Lop Lop { get; set; }

        [XmlElement(ElementName = "windChill")]
        public WindChill WindChill { get; set; }

        [XmlElement(ElementName = "humidex")]
        public Humidex Humidex { get; set; }

        [XmlElement(ElementName = "wind")]
        public Wind Wind { get; set; }

        [XmlAttribute(AttributeName = "dateTimeUTC")]
        public string DateTimeUTC { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "hourlyForecastGroup")]
    public class HourlyForecastGroup
    {

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }

        [XmlElement(ElementName = "hourlyForecast")]
        public List<HourlyForecast> HourlyForecast { get; set; }
    }

    [XmlRoot(ElementName = "precip")]
    public class Precip
    {

        [XmlAttribute(AttributeName = "unitType")]
        public string UnitType { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "yesterdayConditions")]
    public class YesterdayConditions
    {

        [XmlElement(ElementName = "temperature")]
        public List<Temperature> Temperature { get; set; }

        [XmlElement(ElementName = "precip")]
        public Precip Precip { get; set; }
    }

    [XmlRoot(ElementName = "riseSet")]
    public class RiseSet
    {

        [XmlElement(ElementName = "disclaimer")]
        public string Disclaimer { get; set; }

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }
    }

    [XmlRoot(ElementName = "almanac")]
    public class Almanac
    {

        [XmlElement(ElementName = "temperature")]
        public List<Temperature> Temperature { get; set; }

        [XmlElement(ElementName = "precipitation")]
        public List<Precipitation> Precipitation { get; set; }

        [XmlElement(ElementName = "pop")]
        public Pop Pop { get; set; }
    }

    [XmlRoot(ElementName = "siteData")]
    public class SiteData : ISiteData
    {

        [XmlElement(ElementName = "license")]
        public string License { get; set; }

        [XmlElement(ElementName = "dateTime")]
        public List<DateTime> DateTime { get; set; }

        [XmlElement(ElementName = "location")]
        public Location Location { get; set; }

        [XmlElement(ElementName = "warnings")]
        public Warnings Warnings { get; set; }

        [XmlElement(ElementName = "currentConditions")]
        public CurrentConditions CurrentConditions { get; set; }

        [XmlElement(ElementName = "forecastGroup")]
        public ForecastGroup ForecastGroup { get; set; }

        [XmlElement(ElementName = "hourlyForecastGroup")]
        public HourlyForecastGroup HourlyForecastGroup { get; set; }

        [XmlElement(ElementName = "yesterdayConditions")]
        public YesterdayConditions YesterdayConditions { get; set; }

        [XmlElement(ElementName = "riseSet")]
        public RiseSet RiseSet { get; set; }

        [XmlElement(ElementName = "almanac")]
        public Almanac Almanac { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation { get; set; }

        [XmlText]
        public string Text { get; set; }
        public long LastUpdate => long.Parse(CurrentConditions.DateTime.FirstOrDefault()?.TimeStamp);

        public override string ToString()
        {
            return $"{CurrentConditions.Temperature?.Text}{CurrentConditions.Temperature?.Units}({CurrentConditions.WindChill?.Text}) {CurrentConditions.RelativeHumidity?.Text}{CurrentConditions.RelativeHumidity?.Units} {CurrentConditions.Wind?.Speed?.Text}({CurrentConditions.Wind?.Gust?.Text}){CurrentConditions.Wind?.Direction?.Text} {CurrentConditions.Condition} ";
        }
    }
}
