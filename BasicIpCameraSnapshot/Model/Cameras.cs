using System.Collections.Generic;
namespace WebApplication4.Model
{
    public interface ISettings
    {
        int CacheTime { get; set; }
        Dictionary<string, (string credential, string link)> Cameras { get; set; }
    }

    public class Settings : ISettings
    {
        public int CacheTime { get; set; }
        public Dictionary<string, (string credential, string link)> Cameras { get; set; }
    }
}
