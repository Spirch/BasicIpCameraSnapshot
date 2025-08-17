using CommonHelper;

namespace WeatherUpdate.Model;

public sealed class WeatherData
{
    public ISiteData SiteData { get; set; }

    public bool Error { get; set; }
    public string LastDisplay { get; set; }
    public long LastRefresh { get; set; }
    public bool ForceRefresh { get; set; }

    public void Reset()
    {
        SiteData = default;
        Error = default;
        LastDisplay = default;
        LastRefresh = default;
        ForceRefresh = default;
    }

    public override string ToString()
    {
        var result = SiteData.ToString();

        if (Error)
        {
            result = result.Truncate(40) + " Err";
        }

        return result.Truncate();
    }
}