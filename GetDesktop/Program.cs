using CommonHelper;
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