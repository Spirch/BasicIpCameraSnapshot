using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BasicIpCamera.Model;

namespace BasicIpCamera
{
    public class Weather
    {
        private readonly Settings settings;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<Weather> logger;
        private readonly Dictionary<string, WeatherData> weatherDatas;

        public Weather(IOptionsMonitor<Settings> settings, IHttpClientFactory clientFactory, ILogger<Weather> logger, Dictionary<string, WeatherData> weatherDatas)
        {
            this.clientFactory = clientFactory;
            this.settings = settings.CurrentValue;
            this.logger = logger;
            this.weatherDatas = weatherDatas;
        }

        public async Task Refresh()
        {
            var camerasOFF = settings.Cameras.Where(x => !x.Value.WeatherEnable).Select(x => x.Value).ToList();
            var camerasON = settings.Cameras.Where(x => x.Value.WeatherEnable).Select(x => x.Value).ToList();

            foreach(var station in settings.Weather.Stations)
            {
                using var sw = new LogRuntime(logger, $"Processed station {station.Value.Name}");
                logger.LogInformation($"Processing station {station.Value.Name}");

                var weatherData = weatherDatas.Get(station.Value.Name);

                await UpdateOFF(station.Value, camerasOFF, weatherData);
                await UpdateON(station.Value, camerasON, weatherData);
            }
        }

        private async Task UpdateOFF(StationData station, List<Camera> cameras, WeatherData weatherData)
        {
            if (cameras.Count == 0)
            {
                logger.LogInformation($"UpdateOFF No camera to do");
                return;
            }

            await UpdateCameras(station, cameras, nameof(UpdateOFF));
        }


        private async Task UpdateON(StationData station, List<Camera> cameras, WeatherData weatherData)
        {
            if(cameras.Count == 0)
            {
                logger.LogInformation($"UpdateON No camera to do");
                return;
            }

            if(!station.Enable)
            {
                await UpdateOFF(station, cameras, weatherData);
                weatherData.Reset();
                return;
            }

            var data = await GetWeatherData(station, weatherData);

            if(!weatherData.ForceRefresh && weatherData.LastRefresh >= weatherData.LastUpdate)
            {
                logger.LogWarning($"UpdateON Station {station.Name} same or older date {weatherData.LastRefresh} vs {weatherData.LastUpdate}");
                return;
            }

            weatherData.LastRefresh = weatherData.LastUpdate;

            if(!weatherData.ForceRefresh && weatherData.LastDisplay == weatherData.ToString())
            {
                logger.LogWarning($"UpdateON Station {station.Name} same data {weatherData.LastDisplay} vs {weatherData}");
                return;
            }

            weatherData.ForceRefresh = false;
            weatherData.LastDisplay = weatherData.ToString();

            await UpdateCameras(station, cameras, nameof(UpdateON), weatherData.LastDisplay);
        }

        private async Task<string> GetWeatherData(StationData station, WeatherData weatherData)
        {
            string data = "";

            try
            {
                using var sw = new LogRuntime(logger, $"GetWeatherData Finish getting data for station {station.Name}");
                logger.LogInformation($"GetWeatherData Getting data for station {station.Name}");
                bool retry = false;

                while(true)
                    try
                    {
                        using var clientWeather = clientFactory.CreateClient();
                        data = await clientWeather.GetStringAsync(station.Station);
                        break;
                    }
                    catch(Exception ex)
                    {
                        if(retry)
                        {
                            throw;
                        }

                        retry = true;
                        logger.LogError($"GetWeatherData - will retry once - {ex.Message}");
                    }

                ParseWeatherData(station, data, weatherData);

                weatherData.Error = false;
            }
            catch(Exception ex)
            {
                weatherData.Error = true;
                weatherData.ForceRefresh = true;
                logger.LogError($"GetWeatherData {ex.Message}");
            }

            return data;
        }

        private void ParseWeatherData(StationData station, string data, WeatherData weatherData)
        {
            switch(station.Name.ToUpperInvariant())
            {
                case "ECCC":
                    ParseECCC(data, weatherData);
                    break;
                case "THEWEATHERNETWORK":
                    ParseTheWeatherNetwork(data, weatherData);
                    break;
                default:
                    throw new NotSupportedException($"ParseWeatherData of station {station.Name}");
            }
        }

        private void ParseECCC(string data, WeatherData weatherData)
        {
            var weather = XElement.Parse(data);
            var current = weather.Descendants("currentConditions");
            
            weatherData.Condition = current.Select(x => x.Element("condition").Value).FirstOrDefault();
            weatherData.Temperature = current.Select(x => x.Element("temperature").Value).FirstOrDefault();
            weatherData.Humidity = current.Select(x => x.Element("relativeHumidity").Value).FirstOrDefault();

            var date = current.Descendants("dateTime");
            weatherData.LastUpdate = long.Parse(date.Select(x => x.Element("timeStamp").Value).FirstOrDefault());
        }

        private void ParseTheWeatherNetwork(string data, WeatherData weatherData)
        {
            var weather = JsonNode.Parse(data);
            var current = weather["observation"];

            weatherData.Condition = (string)current["weatherCode"]["text"];
            weatherData.Temperature = current["temperature"].ToString();
            weatherData.Humidity = current["relativeHumidity"].ToString();
            weatherData.LastUpdate = long.Parse(DateTime.Parse((string)current["time"]["local"]).ToString("yyyyMMddHHmm"));
        }

        private async Task UpdateCameras(StationData station, List<Camera> cameras, string caller, string lastDisplay = "")
        {
            var enable = lastDisplay != "" ? "true" : "false";
            var content = new StringContent(
                @$"     <?xml version=""1.0"" encoding=""UTF-8""?>
                        <TextOverlayList>
                                <TextOverlay>
                                <id>{station.Line}</id>
                                <enabled>{enable}</enabled>
                                <alignment>0</alignment>
                                <positionX>{station.PosX}</positionX>
                                <positionY>{station.PosY}</positionY>
                                <displayText>{lastDisplay}</displayText>
                            </TextOverlay>
                        </TextOverlayList>
                ");

            using var client = clientFactory.CreateClient();
            
            foreach (var cam in cameras)
            {
                using var sw = new LogRuntime(logger, $"{caller} Updated camera {cam.Name} data for station {station.Name}");
                logger.LogInformation($"{caller} Updating camera {cam.Name} data for station {station.Name}");

                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(cam.Credential));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var response = await client.PutAsync($"{cam.BaseUrl}{cam.Weather}", content);
            }
        }
    }    
}