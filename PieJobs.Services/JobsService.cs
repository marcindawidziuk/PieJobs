using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        Task<JobDto> Get(int jobId);
        Task<int?> GetNextJob();
        Task<JobsService.ProcessResult> ExecuteJob(int id);
        Task SetStatus(int jobId, JobStatus jobStatus);
        Task CancelJobsInProgress();
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

        public async Task<ProcessResult> ExecuteJob(int id)
        {
            await using var db = _contextFactory.Create();

            var job = await db.Jobs.SingleAsync(x => x.Id == id);
            job.StartedDateTimeUtc = DateTime.UtcNow;
            job.Status = JobStatus.InProgress;
            await db.SaveChangesAsync();

            // Start the child process.
            using var process = new Process();
            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = job.Command;
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            Directory.CreateDirectory(Path.GetDirectoryName($"logs/{id}.log"));
            await using var file = File.CreateText($"logs/{id}.log");

            var logs = new List<LogLine>();
            
            var lineNumber = 1;

            process.OutputDataReceived += (_, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;
                
                var log = new LogLine
                {
                    DateTimeUtc = DateTime.UtcNow,
                    Text = args.Data,
                    JobId = id,
                    LineNumber = lineNumber
                };
                logs.Add(log);
            };
            
            process.ErrorDataReceived += (_, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;
                
                var log = new LogLine
                {
                    DateTimeUtc = DateTime.UtcNow,
                    Text = args.Data,
                    JobId = id,
                    LineNumber = lineNumber,
                    IsError = true
                };
                logs.Add(log);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            return new ProcessResult()
            {
                ExitCode = process.ExitCode,
                Logs = logs
            };
        }

        public class ProcessResult
        {
            public int ExitCode { get; set; }
            public List<LogLine> Logs { get; set; }
        }

        public async Task SetStatus(int jobId, JobStatus jobStatus)
        {
            await using var db = _contextFactory.Create();

            var job = await db.Jobs.SingleAsync(x => x.Id == jobId);
            job.Status = jobStatus;
            job.FinishedDateTimeUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        public async Task CancelJobsInProgress()
        {
            await using var db = _contextFactory.Create();

            var jobs = await db.Jobs.Where(x => x.Status == JobStatus.InProgress).ToListAsync();
            
            foreach (var job in jobs)
            {
                job.Status = JobStatus.Cancelled;
                job.FinishedDateTimeUtc = DateTime.UtcNow;
            }

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


        public async Task<JobDto> Get(int jobId)
        {
            await using var db = _contextFactory.Create();

            return await db.Jobs
                .Where(x => x.Id == jobId)
                .Select(x => new JobDto
                {
                    Id = x.Id,
                    JobDefinitionName = x.JobDefinition.Name,
                    Status = x.Status,
                    ScheduleDateTimeUtc = x.ScheduleDateTimeUtc,
                    StartedDateTimeUtc = x.StartedDateTimeUtc,
                    FinishedDateTimeUtc = x.FinishedDateTimeUtc,
                    Command = x.Command
                }).SingleAsync();
        }
        public async Task<List<JobDto>> GetAll(int? maximum)
        {
            await using var db = _contextFactory.Create();

            var jobsQuery = db.Jobs
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