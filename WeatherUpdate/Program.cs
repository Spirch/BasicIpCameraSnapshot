using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using WeatherUpdate;
using WeatherUpdate.Model;

await Host.CreateDefaultBuilder(args)
.ConfigureServices((hostContext, services) =>
{
    IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build();

    services.AddHttpClient("", x =>
    {
        x.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64; rv:105.0) Gecko/20100101 Firefox/105.0");
    });

    services.Configure<Settings>(config.GetSection("AppSettings"));

    services.AddSingleton<Dictionary<string, WeatherData>>();

    services.AddScoped<Weather>();

    services.AddHostedService<RefreshWeather>();
})
.RunConsoleAsync();
