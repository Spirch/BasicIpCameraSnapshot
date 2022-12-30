using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using BasicIpCamera.Model;
using Microsoft.Extensions.Logging;

namespace BasicIpCamera
{
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

    public static class Helper
    {
        private readonly static Regex nameReg = new("^ch\\d{2}_\\d{17}$");

        public static string ToUniversalIso8601(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
        }

        public static string StripUrl(this string url) 
        { 
            return url.Substring(url.IndexOf("?") + 1);
        }

        public static bool ValidName(this string name)
        {
            return nameReg.IsMatch(name);
        }
    }
}