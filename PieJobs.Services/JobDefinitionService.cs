using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PieJobs.Data;

namespace PieJobs.Services
{
    public interface IJobDefinitionService
    {
        Task<int> AddJobDefinition(AddJobDefinitionDto dto);
        Task EditJobDefinition(int jobDefinitionId, AddJobDefinitionDto dto);
        Task DeleteJobDefinition(int jobDefinitionId);
        Task<List<JobDefinitionDto>> GetAll();
        Task<JobDefinitionDto> GetDetails(int id);
    }

    public class JobDefinitionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }

    public class AddJobDefinitionDto
    {
        public string Name { get; set; }
        public string Command { get; set; }
    }
    
    public class JobDefinitionService : IJobDefinitionService
    {
        private readonly IContextFactory _contextFactory;
        
        public JobDefinitionService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> AddJobDefinition(AddJobDefinitionDto dto)
        {
            await using var db = _contextFactory.Create();

            var jobDefinition = new JobDefinition
            {
                Name = dto.Name,
                Command = dto.Command
            };
            db.JobDefinitions.Add(jobDefinition);
            await db.SaveChangesAsync();
            return jobDefinition.Id;
        }

        public async Task EditJobDefinition(int jobDefinitionId, AddJobDefinitionDto dto)
        {
            await using var db = _contextFactory.Create();

            var jobDefinition = await db.JobDefinitions.SingleAsync(x => x.Id == jobDefinitionId);
            jobDefinition.Name = dto.Name;
            jobDefinition.Command = dto.Command;
            jobDefinition.LastModifiedUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

        public async Task DeleteJobDefinition(int jobDefinitionId)
        {
            await using var db = _contextFactory.Create();

            var jobDefinition = await db.JobDefinitions.SingleAsync(x => x.Id == jobDefinitionId);
            db.JobDefinitions.Remove(jobDefinition);

            var jobs = await db.Jobs.Where(x => x.JobDefinitionId == jobDefinitionId).ToListAsync();
            foreach (var job in jobs)
            {
                job.JobDefinitionId = null;
            }
                
            await db.SaveChangesAsync();
        }

        public async Task<List<JobDefinitionDto>> GetAll()
        {
            await using var db = _contextFactory.Create();

            return await db.JobDefinitions
                .Select(x => new JobDefinitionDto
                {
                    Id = x.Id,
                    Command = x.Command,
                    LastModifiedUtc = x.LastModifiedUtc,
                    Name = x.Name
                }).ToListAsync();
        }

        public async Task<JobDefinitionDto> GetDetails(int id)
        {
            await using var db = _contextFactory.Create();

            return await db.JobDefinitions
                .Where(x => x.Id == id)
                .Select(x => new JobDefinitionDto
                {
                    Id = x.Id,
                    Command = x.Command,
                    LastModifiedUtc = x.LastModifiedUtc,
                    Name = x.Name
                }).SingleAsync();
        }
    }
}