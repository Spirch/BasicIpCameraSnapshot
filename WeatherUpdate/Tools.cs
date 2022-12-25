using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

public sealed class LogRuntime : IDisposable
{
    private readonly ILogger logger;
    private readonly string message;
    private Stopwatch sw;

    public LogRuntime(ILogger logger, string message)
    {
        this.logger = logger;
        this.message = message;
        sw = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        logger.LogInformation($"{message} - {sw.Elapsed.TotalMilliseconds}ms");
    }
}

public static class Tools
{
    public static WeatherData Get(this Dictionary<string, WeatherData> weatherDatas, string name)
    {
        if (!weatherDatas.TryGetValue(name, out var weatherData))
        {
            weatherData = new();
            weatherDatas.Add(name, weatherData);
        }

        return weatherData;
    }
}