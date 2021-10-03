using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PieJobs.Services;
using Microsoft.AspNetCore.Mvc;

namespace PieJobs.Api.Controllers
{
    public class JobDefinitionsController : BaseController
    {
        private readonly IJobDefinitionService _jobDefinitionService;
        public JobDefinitionsController(IJobDefinitionService jobDefinitionService)
        {
            _jobDefinitionService = jobDefinitionService;
        }

        [Authorize]
        [HttpPost("add")]
        public Task<int> AddJobDefinition(AddJobDefinitionDto dto)
        {
            return _jobDefinitionService.AddJobDefinition(dto);
        }

        [Authorize]
        [HttpPost("edit")]
        public Task EditJobDefinition(int jobDefinitionId, AddJobDefinitionDto dto)
        {
            return _jobDefinitionService.EditJobDefinition(jobDefinitionId, dto);
        }

        [Authorize]
        [HttpDelete("delete")]
        public Task DeleteJobDefinition(int jobDefinitionId)
        {
            return _jobDefinitionService.DeleteJobDefinition(jobDefinitionId);
        }

        [HttpGet("get-all")]
        public Task<List<JobDefinitionDto>> GetAll()
        {
            return _jobDefinitionService.GetAll();
        }

        [HttpGet("get/{id}")]
        public Task<JobDefinitionDto> GetDetails(int id)
        {
            return _jobDefinitionService.GetDetails(id);
        }
    }
}