using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PieJobs.Services;

namespace PieJobs.Api.Controllers
{
    public class LogsController : BaseController
    {
        private readonly ILogsService _logsService;
        public LogsController(ILogsService logsService)
        {
            _logsService = logsService;
        }

        [HttpGet("get-for-job/{jobId}")]
        public async Task<List<LogLineDto>> GetAll(int jobId)
        {
            return await _logsService.GetLogsForJob(jobId);
        }
    }
}