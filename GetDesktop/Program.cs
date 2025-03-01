using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace WebApplication2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/Get/Desktop", (ILogger<Program> logger, HttpContext httpContext) =>
        {
            using var sw = new LogRuntime(logger, $"GetDesktop IP:{httpContext.Connection.RemoteIpAddress}");

            var mimeType = "image/png";
            return Results.File(DPIUtil.GetDesktopScreen(), contentType: mimeType);
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
