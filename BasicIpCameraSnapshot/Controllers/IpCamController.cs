using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Model;

namespace WebApplication4.Controllers
{
    public class IpCamController : Controller
    {
        static readonly ConcurrentDictionary<string, CachedImage> img = new();

        class CachedImage
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public byte[] Image { get; init; }
        }

        public IActionResult Index([FromServices] ISettings settings)
        {
            return View(settings.Cameras.Keys.ToList());
        }

        public async Task<IActionResult> GetImage([FromServices] ISettings settings, [FromServices] IHttpClientFactory clientFactory, string id, bool thumb = false)
        {
            if (img.TryGetValue(id, out CachedImage cached) && cached.Stopwatch.ElapsedMilliseconds < settings.CacheTime)
                return ProperSize(cached.Image, thumb);

            if (settings.Cameras.TryGetValue(id, out (string credential, string link) info))
            {
                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(info.credential));

                using var client = clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                using var request = new HttpRequestMessage(HttpMethod.Get, info.link);
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
