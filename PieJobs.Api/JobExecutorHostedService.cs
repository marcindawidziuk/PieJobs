using System;
using System.Threading;
using System.Threading.Tasks;
using PieJobs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PieJobs.Data;

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
                    using var scope = _serviceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IJobsService>();
                    var nextJob = await service.GetNextJob();
                    if (nextJob != null)
                    {
                        try
                        {
                            await service.ExecuteJob(nextJob.Value);
                            await service.SetStatus(nextJob.Value, JobStatus.Completed);
                        }
                        catch (Exception ex)
                        {
                            await service.SetStatus(nextJob.Value, JobStatus.Failed);
                            _logger.LogError(ex, "Error occurred executing job {NextJob}", nextJob);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing");
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}