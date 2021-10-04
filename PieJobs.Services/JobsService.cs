using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PieJobs.Data;
using Microsoft.EntityFrameworkCore;

namespace PieJobs.Services
{
    public interface IJobsService
    {
        Task<int> ScheduleJob(int jobDefinitionId);
        Task<List<JobDto>> GetAll(int? maximum);
        Task<int?> GetNextJob();
        Task<string> ExecuteJob(int id);
        Task SetStatus(int jobId, JobStatus jobStatus);
    }

    public class JobDto
    {
        public int Id { get; set; }
        public DateTime ScheduleDateTimeUtc { get; set; }
        public DateTime? StartedDateTimeUtc { get; set; }
        public DateTime? FinishedDateTimeUtc { get; set; }
        public string Command { get; set; }
        public JobStatus Status { get; set; }
        public string JobDefinitionName { get; set; }
    }

    public class JobsService : IJobsService
    {
        private readonly IContextFactory _contextFactory;
        public JobsService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int?> GetNextJob()
        {
            await using var db = _contextFactory.Create();

            var job = await db.Jobs
                .Where(x => x.Status == JobStatus.Pending)
                .OrderBy(x => x.Id)
                .Select(x => new { x.Id })
                .FirstOrDefaultAsync();
            return job?.Id;
        }

        public async Task<string> ExecuteJob(int id)
        {
            await using var db = _contextFactory.Create();

            var job = await db.Jobs.SingleAsync(x => x.Id == id);
            job.StartedDateTimeUtc = DateTime.UtcNow;
            job.Status = JobStatus.InProgress;
            await db.SaveChangesAsync();

            // Start the child process.
            var process = new Process();
            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = job.Command;
            process.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output;
        }

        public async Task SetStatus(int jobId, JobStatus jobStatus)
        {
            await using var db = _contextFactory.Create();

            var job = await db.Jobs.SingleAsync(x => x.Id == jobId);
            job.Status = jobStatus;
            job.FinishedDateTimeUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        public async Task<int> ScheduleJob(int jobDefinitionId)
        {
            await using var db = _contextFactory.Create();
            var jobDefinition = await db.JobDefinitions
                .SingleAsync(x => x.Id == jobDefinitionId);

            var job = new Job
            {
                JobDefinitionId = jobDefinitionId,
                Command = jobDefinition.Command,
                ScheduleDateTimeUtc = DateTime.UtcNow
            };

            db.Jobs.Add(job);
            await db.SaveChangesAsync();
            return job.Id;
        }

        public async Task<List<JobDto>> GetAll(int? maximum)
        {
            await using var db = _contextFactory.Create();

            var jobsQuery =  db.Jobs
                .Select(x => new JobDto
                {
                    Id = x.Id,
                    JobDefinitionName = x.JobDefinition.Name,
                    Status = x.Status,
                    ScheduleDateTimeUtc = x.ScheduleDateTimeUtc,
                    StartedDateTimeUtc = x.StartedDateTimeUtc,
                    FinishedDateTimeUtc = x.FinishedDateTimeUtc,
                    Command = x.Command
                })
                .OrderByDescending(x => x.ScheduleDateTimeUtc);

            if (maximum != null)
            {
                return await jobsQuery.Take(maximum.Value).ToListAsync();
            }
            
            return await jobsQuery.ToListAsync();
        }
    }
}