using System.Collections.Generic;

namespace EventLinkageControl;

public class Settings
{
    public Dictionary<string, Camera> Cameras { get; set; }
}

public sealed class Camera
{
    public string Credential { get; set; }
    public string BaseUrl { get; set; }
    public string Path { get; set; }
    public List<string> Triggers { get; set; }
}