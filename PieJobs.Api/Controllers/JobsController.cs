using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PieJobs.Services;
using Microsoft.AspNetCore.Mvc;

namespace PieJobs.Api.Controllers
{
    public class JobsController : BaseController
    {
        private readonly IJobsService _jobsService;
        public JobsController(IJobsService jobsService)
        {
            _jobsService = jobsService;
        }

        [Authorize]
        [HttpPost("schedule")]
        public async Task<int> ScheduleJob(int jobDefinitionId)
        {
            return await _jobsService.ScheduleJob(jobDefinitionId);
        }
        
        [HttpPost("get-all")]
        public async Task<List<JobDto>> GetAll(int? maximum)
        {
            return await _jobsService.GetAll(maximum);
        }
        
        [HttpPost("get/{jobId}")]
        public async Task<JobDto> Get(int jobId)
        {
            return await _jobsService.Get(jobId);
        }
    }
}