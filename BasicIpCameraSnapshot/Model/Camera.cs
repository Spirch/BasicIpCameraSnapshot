namespace BasicIpCamera.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Credential { get; set; }
        public string BaseUrl { get; set; }
        public string Picture { get; set; }
        public string Weather { get; set; }
        public bool WeatherEnable { get; set; }
    }
}
