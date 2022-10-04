using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BasicIpCamera.Model;

namespace BasicIpCamera.Controllers
{
    public class IpCamController : Controller
    {
        private static readonly ConcurrentDictionary<string, CachedImage> img = new();
        private readonly Settings settings;
        private readonly ILogger<IpCamController> logger;


        public IpCamController(IOptionsMonitor<Settings> settings, ILogger<IpCamController> logger)
        {
            this.settings = settings.CurrentValue;
            this.logger = logger;
        }

        private class CachedImage
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public byte[] Image { get; init; }
        }

        public async Task<IActionResult> Index()
        {
            using var sw = new LogRuntime(logger, $"Index IP:{Request.HttpContext.Connection.RemoteIpAddress}");
            
            await Task.CompletedTask;
            return View(settings.Cameras.Keys.ToList());
        }

        public async Task<IActionResult> GetImage([FromServices] IHttpClientFactory clientFactory, string id, bool thumb = false)
        {
            using var sw = new LogRuntime(logger, $"GetImage {nameof(id)}:{id} {nameof(thumb)}:{thumb} IP:{Request.HttpContext.Connection.RemoteIpAddress}");

            return await ProcessGetImage(clientFactory, id, thumb);
        }

        private async Task<IActionResult> ProcessGetImage(IHttpClientFactory clientFactory, string id, bool thumb = false)
        {
            if (img.TryGetValue(id, out CachedImage cached) && cached.Stopwatch.ElapsedMilliseconds < settings.CacheTime)
                return ProperSize(cached.Image, thumb);

            if (settings.Cameras.TryGetValue(id, out Camera info))
            {
                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(info.Credential));

                using var client = clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{info.BaseUrl}{info.Picture}");
                using var response = await client.SendAsync(request);

                cached = new CachedImage()
                {
                    Image = await response.Content.ReadAsByteArrayAsync()
                };

                img[id] = cached;

                return ProperSize(cached.Image, thumb);
            }

            return NotFound();
        }

        private FileContentResult ProperSize(byte[] rawImage, bool thumb)
        {
            byte[] resizedRawImage = null;

            if (thumb)
            {
                using var original = SKBitmap.Decode(rawImage);
                var resizeInfo = new SKImageInfo(original.Width / 4, original.Height / 4);

                using var resized = original.Resize(resizeInfo, SKFilterQuality.High);
                using var rawData = resized.Encode(SKEncodedImageFormat.Jpeg, 90);

                resizedRawImage = rawData.ToArray();
            }

            return File(resizedRawImage ?? rawImage, "image/jpg");
        }
    }
}
