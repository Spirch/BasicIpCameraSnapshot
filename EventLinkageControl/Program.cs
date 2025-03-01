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
    private static readonly ConcurrentDictionary<string, string> eventXml = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpClient();

        var app = builder.Build();

        app.MapGet("linkage/{groupName}/{time?}", async (ILogger<Program> logger, HttpContext httpContext,
                                                         IHttpClientFactory httpClientFactory, IConfiguration config,
                                                         string groupName, int? time) =>
        {
            using var sw = new LogRuntime(logger, $"linkage {groupName}/{time} IP:{httpContext.Connection.RemoteIpAddress}");

            if (ValidateParams(config, groupName, time, out var args))
            {
                var actions = await PrepareChangeLinkage(config, args.groups, args.action);

                await ChangeLinkage(httpClientFactory, actions);

                return "Ok";
            }

            return "Not Ok";
        });

        app.Run();
    }

    private static bool ValidateParams(IConfiguration config, string groupName, int? time, out (List<string> groups, string action, int? time) args)
    {

        if (time.HasValue && (time < 1 || time > 120))
        {
            args = new();
            return false;
        }

        var action = time.HasValue ? "off" : "on";

        var group = config.GetSection("Group").Get<Dictionary<string, List<string>>>();

        if (group.TryGetValue(groupName, out var groups))
        {
            args = (groups, action, time);
            return true;
        }

        args = new();
        return false;
    }

    private static async Task<Dictionary<Camera, List<(string, string)>>> PrepareChangeLinkage(IConfiguration config, List<string> groups, string action)
    {
        var cameras = config.GetSection("Cameras").Get<Dictionary<string, Camera>>();
        var actions = new Dictionary<Camera, List<(string, string)>>();

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

    private static async Task ChangeLinkage(IHttpClientFactory httpClientFactory, Dictionary<Camera, List<(string, string)>> actions)
    {
        Parallel.ForEach(actions, async src =>
        {
            var httpClient = httpClientFactory.CreateClient();
            string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(src.Key.Credential));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            var baseUri = $"{src.Key.BaseUrl}{src.Key.Path}";

            foreach (var trigger in src.Value)
            {
                var uri = baseUri + trigger.Item1 + "-1";
                var content = new StringContent(trigger.Item2, Encoding.UTF8, "application/x-www-form-urlencoded");
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

public class Settings
{
    public Dictionary<string, Camera> Cameras { get; set; }
}

public sealed class Camera
{
    public string Credential { get; set; }
    public string BaseUrl { get; set; }
    public string Path { get; set; }
    public List<string> Triggers { get; set; }
}


public static class EventTriggerFactory
{
    public static EventTrigger NewEventTrigger(string eventType, string action)
    {
        return new EventTrigger()
        {
            Id = eventType + "-1",
            EventType = eventType,
            EventTriggerNotificationList = new()
            {
                EventTriggerNotification = string.Equals(action, "off", StringComparison.OrdinalIgnoreCase) ?
                new()
                {
                    new() { Id = "center", NotificationMethod = "center" },
                    new() { Id = "FTP", NotificationMethod = "FTP" },
                } :
                new()
                {
                    new() { Id = "email", NotificationMethod = "email" },
                    new() { Id = "center", NotificationMethod = "center" },
                    new() { Id = "FTP", NotificationMethod = "FTP" },
                },
            }
        };
    }
}


[XmlRoot(ElementName = "EventTriggerNotification")]
public class EventTriggerNotification
{

    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "notificationMethod")]
    public string NotificationMethod { get; set; }
}

[XmlRoot(ElementName = "EventTriggerNotificationList")]
public class EventTriggerNotificationList
{

    [XmlElement(ElementName = "EventTriggerNotification")]
    public List<EventTriggerNotification> EventTriggerNotification { get; set; }
}

[XmlRoot(ElementName = "EventTrigger")]
public class EventTrigger
{

    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "eventType")]
    public string EventType { get; set; }

    [XmlElement(ElementName = "videoInputChannelID")]
    public int VideoInputChannelID { get; set; } = 1;

    [XmlElement(ElementName = "EventTriggerNotificationList")]
    public EventTriggerNotificationList EventTriggerNotificationList { get; set; }
}

