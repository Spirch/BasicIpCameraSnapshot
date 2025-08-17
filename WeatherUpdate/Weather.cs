using CommonHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WeatherUpdate.Model;

namespace WeatherUpdate;

public sealed class Weather
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

    public async Task Refresh(CancellationToken ct)
    {
        var camerasOFF = settings.Cameras.Where(x => !x.Value.WeatherEnable).Select(x => x.Value).ToList();
        var camerasON = settings.Cameras.Where(x => x.Value.WeatherEnable).Select(x => x.Value).ToList();

        var camData = new List<(string cam, string content)>();

        foreach (var station in settings.Weather.Stations)
        {
            using var sw = new LogRuntime(logger, $"Processed station {station.Value.Name}");
            logger.LogInformation($"Processing station {station.Value.Name}");

            if (!weatherDatas.TryGetValue(station.Value.Name, out var weatherData))
            {
                weatherData = new();
                weatherDatas.Add(station.Value.Name, weatherData);
            }

            MergeCamData(camData, UpdateOFF(station.Value, camerasOFF, weatherData));
            MergeCamData(camData, await UpdateON(station.Value, camerasON, weatherData));
        }

        await UpdateCameras(camData, ct);
    }

    private async Task UpdateCameras(List<(string cam, string content)> camData, CancellationToken ct)
    {
        if (camData.Count == 0)
        {
            return;
        }

        var updates = (from cam in camData.GroupBy(x => x.cam)
                       join setting in settings.Cameras.Values on cam.Key equals setting.Name
                       select new
                       {
                           cam,
                           setting
                       }).ToList();

        await Parallel.ForEachAsync(updates, async (update, ct) =>
        {
            using var sw = new LogRuntime(logger, $"UpdateCameras Updated camera {update.setting.Name}");

            string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(update.setting.Credential));
            var data = string.Join("", update.cam.Select(x => x.content));
            var content = new StringContent(@$"<?xml version=""1.0"" encoding=""UTF-8""?><TextOverlayList>{data}</TextOverlayList>");

            using var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            using var response = await client.PutAsync($"{update.setting.BaseUrl}{update.setting.Weather}", content);
        });
    }

    private static void MergeCamData(List<(string cam, string content)> camData, List<(string cam, string content)> cams)
    {
        if (cams?.Count > 0)
        {
            camData.AddRange(cams);
        }
    }

    private List<(string cam, string content)> UpdateOFF(StationData station, List<Camera> cameras, WeatherData weatherData)
    {
        if (cameras.Count == 0)
        {
            logger.LogInformation($"UpdateOFF No camera to do");
            return default;
        }

        return CamContent(station, cameras, nameof(UpdateOFF));
    }

    private async Task<List<(string cam, string content)>> UpdateON(StationData station, List<Camera> cameras, WeatherData weatherData)
    {
        if (cameras.Count == 0)
        {
            logger.LogInformation($"UpdateON No camera to do");
            return default;
        }

        if (!station.Enable)
        {
            var explicitOff = UpdateOFF(station, cameras, weatherData);
            weatherData.Reset();
            return explicitOff;
        }

        var data = await GetWeatherData(station, weatherData);

        if (!weatherData.ForceRefresh && weatherData.LastRefresh >= weatherData.SiteData.LastUpdate)
        {
            logger.LogWarning($"UpdateON Station {station.Name} same or older date {weatherData.LastRefresh} vs {weatherData.SiteData.LastUpdate}");
            return default;
        }

        weatherData.LastRefresh = weatherData.SiteData.LastUpdate;

        if (!weatherData.ForceRefresh && weatherData.LastDisplay == weatherData.ToString())
        {
            logger.LogWarning($"UpdateON Station {station.Name} same data {weatherData.LastDisplay} vs {weatherData}");
            return default;
        }

        weatherData.ForceRefresh = false;
        weatherData.LastDisplay = weatherData.ToString();

        return CamContent(station, cameras, nameof(UpdateON), weatherData);
    }

    private async Task<string> GetWeatherData(StationData station, WeatherData weatherData)
    {
        string data = "";

        try
        {
            using var sw = new LogRuntime(logger, $"GetWeatherData Finish getting data for station {station.Name}");
            logger.LogInformation($"GetWeatherData Getting data for station {station.Name}");
            bool retry = false;

            while (true)
            {
                try
                {
                    using var clientWeather = clientFactory.CreateClient();
                    data = await clientWeather.GetStringAsync(station.Station);
                    break;
                }
                catch (Exception ex)
                {
                    if (retry)
                    {
                        throw;
                    }

                    retry = true;
                    logger.LogError($"GetWeatherData - will retry once - {ex.Message}");
                    await Task.Delay(500); //wait half a second
                }
            }

            ParseWeatherData(station, data, weatherData);

            weatherData.Error = false;
        }
        catch (Exception ex)
        {
            weatherData.Error = true;
            weatherData.ForceRefresh = true;
            logger.LogError($"GetWeatherData {ex.Message}");
        }

        return data;
    }

    private static void ParseWeatherData(StationData station, string data, WeatherData weatherData)
    {
        switch (station.Name.ToUpperInvariant())
        {
            case "ECCC_JSON":
                weatherData.SiteData = JsonSerializer.Deserialize<List<Model.ECCC_JSON.Root>>(data).FirstOrDefault();
                break;
            case "THEWEATHERNETWORK":
                weatherData.SiteData = JsonSerializer.Deserialize<Model.WeatherNetwork.SiteData>(data);
                break;
            default:
                throw new NotSupportedException($"ParseWeatherData of station {station.Name}");
        }
    }

    private List<(string cam, string content)> CamContent(StationData station, List<Camera> cameras, string caller, WeatherData data = null)
    {
        var camData = new List<(string cam, string content)>();

        var enable = data != null ? "true" : "false";
        var content =
@$"<TextOverlay><id>{station.Line}</id><enabled>{enable}</enabled><alignment>0</alignment><positionX>{station.PosX}</positionX><positionY>{station.PosY}</positionY><displayText>{data?.LastDisplay}</displayText></TextOverlay>";

        foreach (var cam in cameras)
        {
            logger.LogInformation($"{caller} Updating camera {cam.Name} data for station {station.Name}");
            camData.Add((cam.Name, content));
        }

        return camData;
    }
}