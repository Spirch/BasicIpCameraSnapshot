namespace BasicIpCamera.Model
{
    public class WeatherData
    {
        public string Condition { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public bool Error { get; set; }
        public long LastUpdate { get; set; }
        public string LastDisplay { get; set; }
        public long LastRefresh { get; set; }
        public bool ForceRefresh  { get; set; }

        public void Reset()
        {
            Condition = default;
            Temperature = default;
            Humidity = default;
            Error = default;
            LastUpdate = default;
            LastDisplay = default;
            LastRefresh = default;
            ForceRefresh = default;
        }
        
        public override string ToString()
        {
            if(Error)
            {
                return $"{Temperature}c {Humidity}% {Condition} {Error}  ";
            }

            return $"{Temperature}c {Humidity}% {Condition} ";
        }
    }    
}