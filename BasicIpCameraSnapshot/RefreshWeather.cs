using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BasicIpCamera.Model;

namespace BasicIpCamera
{
    public class RefreshWeather : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDisposable _settingsChangedListener;
        private Settings _settings;
        private PeriodicTimer timer;
        
        public RefreshWeather(IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<Settings> settings) 
        {
            _serviceScopeFactory = serviceScopeFactory;
            _settings = settings.CurrentValue;
            _settingsChangedListener = settings.OnChange(MyOptionsChanged);
        }

        private void MyOptionsChanged(Settings settings, string arg2)
        {
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int lastInterval = int.MinValue;

            if(_settings.Weather.DelayBeforeStart > 0)
                await Task.Delay(_settings.Weather.DelayBeforeStart * 1000); //wait a little bit before starting the process

            do
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetService<ILogger<RefreshWeather>>();
                    var weather = scope.ServiceProvider.GetRequiredService<Weather>();

                    try
                    {

                        using var sw = new LogRuntime(logger, $"Executed");
                        logger.LogInformation($"Executing {DateTime.Now}");
                        await weather.Refresh();
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex.Message);
                        //do nothing
                    }
                }

                lastInterval = CheckInterval(lastInterval);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private int CheckInterval(int lastInterval)
        {
            if(lastInterval != _settings.Weather.Interval)
            {
                lastInterval = _settings.Weather.Interval;
                
                timer?.Dispose();
                timer = new PeriodicTimer(TimeSpan.FromMinutes(lastInterval));
            }

            return lastInterval;
        }

        public override void Dispose()
        {
            timer?.Dispose();
            _settingsChangedListener?.Dispose();

            base.Dispose();
        }
    }    
}