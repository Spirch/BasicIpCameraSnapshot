using CommonHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace EventLinkageControl;

public class Program
{
    private static readonly HashSet<string> validAction = ["on", "off"];

    private static readonly ConcurrentDictionary<string, string> eventXml = new();

    //private static readonly ConcurrentDictionary<string, >

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpClient();

        var app = builder.Build();

        app.MapGet("linkage/{groupName}/{action}/{time?}", async (ILogger<Program> logger, HttpContext httpContext,
                                                                  IHttpClientFactory httpClientFactory, IConfiguration config,
                                                                  string groupName, string action, int? time) =>
        {
            using var sw = new LogRuntime(logger, $"linkage {groupName}/{time} IP:{httpContext.Connection.RemoteIpAddress}");

            if (ValidateParams(config, groupName, action, time, out var args))
            {
                var actions = await PrepareChangeLinkage(config, args.groups, args.action);

                await ChangeLinkage(httpClientFactory, actions);

                await HandleTime(args);

                return Results.Ok();
            }

            return Results.NotFound();
        });

        app.Run();
    }

    private static async Task HandleTime((List<string> groups, string action, int? time) args)
    {
        if(args.time.HasValue && args.time > 0)
        {

        }

        await Task.CompletedTask;
    }

    private static bool ValidateParams(IConfiguration config, string groupName, string action, int? time, out (List<string> groups, string action, int? time) args)
    {

        if (time.HasValue && (time < 0 || time > 240) || !validAction.Contains(action))
        {
            args = new();
            return false;
        }

        var group = config.GetSection("Group").Get<Dictionary<string, List<string>>>();

        if (group.TryGetValue(groupName, out var groups))
        {
            args = (groups, action, time);
            return true;
        }

        args = new();
        return false;
    }

    private static async Task<Dictionary<Camera, List<(string trigger, string xml)>>> PrepareChangeLinkage(IConfiguration config, List<string> groups, string action)
    {
        var cameras = config.GetSection("Cameras").Get<Dictionary<string, Camera>>();
        var actions = new Dictionary<Camera, List<(string trigger, string xml)>>();

        foreach (var cam in cameras.Where(x => groups.Contains(x.Key)).Select(x => x.Value))
        {
            actions.Add(cam, new());

            foreach (var trigger in cam.Triggers)
            {
                var xml = eventXml.GetOrAdd(string.Concat(trigger, action), _ => SerializeToXml(EventTriggerFactory.NewEventTrigger(trigger, action)));
                actions[cam].Add((trigger, xml));
            }
        }

        await Task.CompletedTask;

        return actions;
    }

    private static async Task ChangeLinkage(IHttpClientFactory httpClientFactory, Dictionary<Camera, List<(string trigger, string xml)>> actions)
    {
        Parallel.ForEach(actions, async src =>
        {
            var httpClient = httpClientFactory.NewBasicCamHttpClient(src.Key.Credential);
            var baseUri = $"{src.Key.BaseUrl}{src.Key.Path}";

            foreach (var trigger in src.Value)
            {
                var uri = baseUri + trigger.trigger + "-1";
                var content = new StringContent(trigger.xml, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await httpClient.PutAsync(uri, content);
                result.EnsureSuccessStatusCode();

                Console.WriteLine(result);

                await Task.Delay(2000);
            }
        });

        await Task.CompletedTask;
    }

    private static string SerializeToXml<T>(T obj)
    {
        var serializer = new XmlSerializer(typeof(T));
        var ns = new XmlSerializerNamespaces();
        ns.Add("", ""); // Remove XML namespaces

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        serializer.Serialize(xmlWriter, obj, ns);
        return stringWriter.ToString();
    }
}
