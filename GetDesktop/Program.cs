using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GetDesktop;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/Get/Desktop", async (ILogger<Program> logger, HttpContext httpContext) =>
        {
            using var sw = new LogRuntime(logger, $"GetDesktop IP:{httpContext.Connection.RemoteIpAddress}");

            var mimeType = "image/png";
            var stream = new MemoryStream();
            DPIUtil.GetDesktopScreen(stream);

            await Task.CompletedTask;

            return Results.File(stream, contentType: mimeType);
        });

        app.Run();
    }
}
public class LogRuntime : IDisposable
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
