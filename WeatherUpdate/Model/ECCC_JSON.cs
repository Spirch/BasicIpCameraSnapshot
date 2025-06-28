using System;
using System.Text.Json.Serialization;

namespace WeatherUpdate.Model.ECCC_JSON;
public class Dewpoint
{
    public string imperial { get; set; }
    public string imperialUnrounded { get; set; }
    public string metric { get; set; }
    public string metricUnrounded { get; set; }
    public int qaValue { get; set; }
}

public class FeelsLike
{
    public string imperial { get; set; }
    public string metric { get; set; }
}

public class Observation
{
    public string observedAt { get; set; }
    public string provinceCode { get; set; }
    public string climateId { get; set; }
    public string tcid { get; set; }
    public DateTime timeStamp { get; set; }
    public string timeStampText { get; set; }
    public string iconCode { get; set; }
    public string condition { get; set; }
    public Temperature temperature { get; set; }
    public Dewpoint dewpoint { get; set; }
    public FeelsLike feelsLike { get; set; }
    public Pressure pressure { get; set; }
    public string tendency { get; set; }
    public Visibility visibility { get; set; }
    public double visUnround { get; set; }
    public string humidity { get; set; }
    public int humidityQaValue { get; set; }
    public WindSpeed windSpeed { get; set; }
    public WindGust windGust { get; set; }
    public string windDirection { get; set; }
    public int windDirectionQAValue { get; set; }
    public string windBearing { get; set; }
}

public class Pressure
{
    public string imperial { get; set; }
    public string metric { get; set; }
    public string changeImperial { get; set; }
    public string changeMetric { get; set; }
    public int qaValue { get; set; }
}

public class Root : ISiteData
{
    public long lastUpdated { get; set; }
    public Observation observation { get; set; }

    [JsonIgnore]
    public long LastUpdate => lastUpdated;

    public override string ToString()
    {
        return $"{observation.temperature.metric}C({observation.feelsLike.metric}) {observation.humidity}% {observation.windSpeed.metric}({observation.windGust.metric}){observation.windDirection} {observation.condition} ";
    }
}

public class Temperature
{
    public string imperial { get; set; }
    public string imperialUnrounded { get; set; }
    public string metric { get; set; }
    public string metricUnrounded { get; set; }
    public int qaValue { get; set; }
    public int periodLow { get; set; }
    public int? periodHigh { get; set; }
}

public class Visibility
{
    public string imperial { get; set; }
    public string metric { get; set; }
    public int qaValue { get; set; }
}

public class WindGust
{
    public string imperial { get; set; }
    public string metric { get; set; }
}

public class WindSpeed
{
    public string imperial { get; set; }
    public string metric { get; set; }
    public int qaValue { get; set; }
}

