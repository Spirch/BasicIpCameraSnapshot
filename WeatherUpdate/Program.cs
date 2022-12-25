using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

await Host.CreateDefaultBuilder(args)
.ConfigureServices((hostContext, services) =>
{
    IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSimpleConsole(formatterOptions =>
        {
            formatterOptions.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff   ";
            formatterOptions.SingleLine = true;
        });
    });

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
