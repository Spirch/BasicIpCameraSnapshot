namespace WeatherUpdate.Model;

public sealed class Camera
{
    public string Name { get; set; }
    public string Credential { get; set; }
    public string BaseUrl { get; set; }
    public string Weather { get; set; }
    public bool WeatherEnable { get; set; }
}