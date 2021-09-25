using System;
using System.Threading;
using System.Threading.Tasks;
using PieJobs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PieJobs.Api
{
    public class JobExecutorHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobExecutorHostedService> _logger;

        public JobExecutorHostedService(IServiceProvider serviceProvider, ILogger<JobExecutorHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var service = _serviceProvider.GetRequiredService<JobsService>();
                    var nextJob = await service.GetNextJob();
                    if (nextJob != null)
                    {
                        await service.ExecuteJob(nextJob.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing");
                }
                
                await Task.Delay(10, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}