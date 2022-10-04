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
            var camerasON = settings.Cameras.Where(x => x.Value.WeatherEnable).Select(x => x.Value).ToList();
            var camerasOFF = settings.Cameras.Where(x => !x.Value.WeatherEnable).Select(x => x.Value).ToList();

            foreach(var station in settings.Weather.Stations)
            {
                logger.LogInformation($"Processing station {station.Value.Name}");

                await UpdateOFF(station.Value, camerasOFF);
                await UpdateON(station.Value, camerasON);
            }
        }

        private async Task UpdateON(StationData station, List<Camera> cameras)
        {
            if(cameras.Count == 0)
            {
                logger.LogInformation($"UpdateON No camera to do");
                return;
            }

            if(!weatherDatas.TryGetValue(station.Name, out var weatherData))
            {
                weatherData = new();
                weatherDatas.Add(station.Name, weatherData);
            }

            if(!station.Enable)
            {
                await UpdateOFF(station, cameras);
                weatherData.LastUpdate = 0;
                weatherData.LastDisplay = "";
                return;
            }

            try
            {
                logger.LogInformation($"UpdateON Getting data for station {station.Name}");
                using var clientWeather = clientFactory.CreateClient();
                var data = await clientWeather.GetStringAsync(station.Station);

                if(data[0] == '<')
                {
                    var weather = XElement.Parse(data);
                    var current = weather.Descendants("currentConditions");
                    
                    weatherData.Condition = current.Select(x => x.Element("condition").Value).FirstOrDefault();
                    weatherData.Temperature = current.Select(x => x.Element("temperature").Value).FirstOrDefault();
                    weatherData.Humidity = current.Select(x => x.Element("relativeHumidity").Value).FirstOrDefault();

                    var date = current.Descendants("dateTime");
                    weatherData.LastUpdate = long.Parse(date.Select(x => x.Element("timeStamp").Value).FirstOrDefault());
                }
                else if(data[0] == '{')
                {
                    var weather = JsonNode.Parse(data);
                    var current = weather["observation"];

                    weatherData.Condition = (string)current["weatherCode"]["text"];
                    weatherData.Temperature = current["temperature"].ToString();
                    weatherData.Humidity = current["relativeHumidity"].ToString();
                    weatherData.LastUpdate = long.Parse(DateTime.Parse((string)current["time"]["local"]).ToString("yyyyMMddHHmm"));
                }

                weatherData.Error = false;
            }
            catch(Exception ex)
            {
                weatherData.Error = true;
                logger.LogError($"UpdateON {ex.Message}");
            }

            if(weatherData.LastRefresh >= weatherData.LastUpdate)
            {
                logger.LogWarning($"UpdateON Station {station.Name} same date");
                return;
            }

            weatherData.LastRefresh = weatherData.LastUpdate;

            if(weatherData.LastDisplay == weatherData.ToString())
            {
                logger.LogWarning($"UpdateON Station {station.Name} same data");
                return;
            }

            weatherData.LastDisplay = weatherData.ToString();

            using var clientCamera = clientFactory.CreateClient();
            var content = new StringContent(
                @$"     <?xml version=""1.0"" encoding=""UTF-8""?>
                        <TextOverlayList>
                                <TextOverlay>
                                <id>{station.Line}</id>
                                <enabled>true</enabled>
                                <alignment>0</alignment>
                                <positionX>{station.PosX}</positionX>
                                <positionY>{station.PosY}</positionY>
                                <displayText>{weatherData.LastDisplay}</displayText>
                            </TextOverlay>
                        </TextOverlayList>
                ");

            foreach(var cam in cameras)
            {
                logger.LogInformation($"UpdateON Updating camera {cam.Name} data for station {station.Name}");
                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(cam.Credential));
                clientCamera.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var response = await clientCamera.PutAsync($"{cam.BaseUrl}{cam.Weather}", content);
            }
        }

        private async Task UpdateOFF(StationData station, List<Camera> cameras)
        {
            if(cameras.Count == 0)
            {
                logger.LogInformation($"UpdateOFF No camera to do");
                return;
            }

            using var client = clientFactory.CreateClient();
            var content = new StringContent(
                @$"     <?xml version=""1.0"" encoding=""UTF-8""?>
                        <TextOverlayList>
                                <TextOverlay>
                                <id>{station.Line}</id>
                                <enabled>false</enabled>
                            </TextOverlay>
                        </TextOverlayList>
                ");

            foreach(var cam in cameras)
            {
                logger.LogInformation($"UpdateOFF Updating camera {cam.Name} data for station {station.Name}");

                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(cam.Credential));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var response = await client.PutAsync($"{cam.BaseUrl}{cam.Weather}", content);
            }
        }
    }    
}