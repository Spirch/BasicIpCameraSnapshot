﻿using BasicIpCamera.Model;
using BasicIpCameraSnapshot.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

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

        public async Task<IActionResult> GetHistory([FromServices] IHttpClientFactory clientFactory, string id, string start = null, string end = null, int max = 10, int page = 0)
        {
            using var sw = new LogRuntime(logger, $"GetHistory {nameof(id)}:{id} {nameof(start)}:{start} {nameof(end)}:{end} {nameof(max)}:{max} {nameof(page)}:{page} IP:{Request.HttpContext.Connection.RemoteIpAddress}");

            List<NameValueCollection> result = default;
            DateTime startTime;
            DateTime endTime;

            if (start == null)
            {
                start = DateTime.Now.AddHours(-1).ToUniversalIso8601();
            }

            if (DateTime.TryParse(start, out startTime))
            {
                if(end == null || !DateTime.TryParse(end, out endTime))
                {
                    endTime = startTime.AddDays(1);
                }
            }
            else
            {
                return new BadRequestResult();
            }

            if (max < 0 || max > 20) max = 3;
            if (page < 0 || page > 5) page = 0;

            if (settings.Cameras.TryGetValue(id, out Camera info))
            {
                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(info.Credential));
                using var client = clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{info.BaseUrl}{info.Search}");
                request.Content = new StringContent($"<CMSearchDescription><searchID>{Guid.NewGuid()}</searchID><trackList><trackID>{info.TrackID}</trackID></trackList><timeSpanList><timeSpan><startTime>{startTime.ToUniversalIso8601()}</startTime><endTime>{endTime.ToUniversalIso8601()}</endTime></timeSpan></timeSpanList><maxResults>{max}</maxResults><searchResultPostion>{page}</searchResultPostion><metadataList><metadataDescriptor>//recordType.meta.std-cgi.com</metadataDescriptor></metadataList></CMSearchDescription>");
                using var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(CMSearchResult));
                CMSearchResult searchResult = default;

                using (StringReader reader = new StringReader(content))
                {
                    searchResult = (CMSearchResult)serializer.Deserialize(reader);
                }

                if(searchResult?.MatchList?.SearchMatchItem?.Count> 0) 
                {
                    result = searchResult.MatchList.SearchMatchItem.Select(x => HttpUtility.ParseQueryString(x.MediaSegmentDescriptor.PlaybackURI.StripUrl())).ToList();
                }
            }

            return View((id, result));
        }

        

        public async Task<FileStreamResult> GetPlayBack([FromServices] IHttpClientFactory clientFactory, string id, string starttime, string endtime, string name, int size)
        {
            using var sw = new LogRuntime(logger, $"GetHistory {nameof(id)}:{id} {nameof(starttime)}:{starttime} {nameof(endtime)}:{endtime} {nameof(name)}:{name} {nameof(size)}:{size} IP:{Request.HttpContext.Connection.RemoteIpAddress}");
            CultureInfo culture = CultureInfo.InvariantCulture;

            if (!DateTime.TryParseExact(starttime, "yyyyMMddTHHmmssZ", culture, DateTimeStyles.None, out var _) || 
                !DateTime.TryParseExact(endtime, "yyyyMMddTHHmmssZ", culture, DateTimeStyles.None, out var _) ||
                !name.ValidName())
            {
                throw new Exception();
            }

            if (settings.Cameras.TryGetValue(id, out Camera info))
            {
                string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(info.Credential));
                using var client = clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{info.BaseUrl}{info.Download}");
                request.Content = new StringContent($"<downloadRequest><playbackURI>{info.PlaybackUrl}/?starttime={starttime}&amp;endtime={endtime}&amp;name={name}&amp;size={size}</playbackURI></downloadRequest>");
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                Response.ContentLength = response.Content.Headers.ContentLength;

                return File(response.Content.ReadAsStream(), "application/octet-stream", $"{starttime}_{endtime}.mp4");
            }

            throw new Exception();
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
