using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PieJobs.Data;

namespace PieJobs.Services
{
    public interface ILogsService
    {
        Task<List<LogLineDto>> GetLogsForJob(int jobId);
        Task SaveLogs(IEnumerable<LogLine> logLines);
    }

    public class LogLineDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public bool IsError { get; set; }
        public int LineNumber { get; set; }
    }
    
    public class LogsService : ILogsService
    {
        private readonly IContextFactory _contextFactory;

        public LogsService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<LogLineDto>> GetLogsForJob(int jobId)
        {
            await using var db = _contextFactory.Create();

            return await db.LogLines
                .Where(x => x.JobId == jobId)
                .Select(x => new LogLineDto
                {
                    Id = x.Id,
                    LineNumber = x.LineNumber,
                    IsError = x.IsError,
                    DateTimeUtc = x.DateTimeUtc,
                    Text = x.Text
                })
                .OrderBy(x => x.LineNumber)
                .ToListAsync();
        }

        public async Task SaveLogs(IEnumerable<LogLine> logLines)
        {
            await using var db = _contextFactory.Create();

            await db.LogLines.AddRangeAsync(logLines);
            
            await db.SaveChangesAsync();
        }

        
    }
}