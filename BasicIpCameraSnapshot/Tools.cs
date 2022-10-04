using System;
using System.Diagnostics;
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

    public static class Tools
    {
        public static void LogInfo(this ILogger logger)
        {
            var sw = Stopwatch.StartNew();
            var message = $"";
            try
            {

            }
            finally
            {
                logger.LogInformation($"{message} - {sw.Elapsed.TotalMilliseconds}ms");
            }
        }
    }
}