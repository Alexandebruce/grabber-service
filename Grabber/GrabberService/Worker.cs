using System;
using System.Threading;
using System.Threading.Tasks;
using GrabberService.Service.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrabberService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IGismeteoParser gismeteoParser;

        public Worker(ILogger<Worker> logger, IGismeteoParser gismeteoParser)
        {
            this.logger = logger;
            this.gismeteoParser = gismeteoParser;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    await gismeteoParser.Do();
                }
                catch (Exception ex)
                {
                    logger.LogError("Worker exception: {time} {ex}", DateTimeOffset.Now, ex);
                }
                finally
                {
                    await Task.Delay(60 * 60 * 1000, stoppingToken);//60 * 60 * 1000 = one hour in milliseconds
                }
            }
        }
    }
}