using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PTZControl;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddHttpClient();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        var app = builder.Build();

        var camera = app.Configuration.GetSection("Camera").Get<List<Camera>>();

        foreach (var cam in camera)
        {
            foreach (var preset in cam.Presets)
            {
                app.MapGet($"ptz/{cam.Name}/{preset.Name}", async (IHttpClientFactory httpClientFactory) =>
                {
                    var httpClient = httpClientFactory.CreateClient();
                    string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(cam.Credential));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                    var uri = $"http://{cam.IP}/ISAPI/PTZCtrl/channels/1/presets/{preset.Id}/goto";
                    var result = await httpClient.PutAsync(uri, null);
                    result.EnsureSuccessStatusCode();

                    await Task.Delay(cam.WaitForRedirect * 1000);

                    return Results.Redirect(cam.RedirectUri);
                });
            }
        }

        app.Run();
    }
}

public class Camera
{
    public string Name { get; set; }
    public string Credential { get; set; }
    public string IP { get; set; }
    public string RedirectUri { get; set; }
    public int WaitForRedirect { get; set; }
    public List<Preset> Presets { get; set; }
}

public class Preset
{
    public string Name { get; set; }
    public int Id { get; set; }
}

[JsonSerializable(typeof(HttpResponseMessage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}