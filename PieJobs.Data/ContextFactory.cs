using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PieJobs.Data
{
    public interface IContextFactory
    {
        ApplicationDbContext Create();
    }
    
    public class ContextFactory : IContextFactory
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ContextFactory(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ApplicationDbContext Create()
        {
            return _contextFactory.CreateDbContext();
        }
    }
    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobDefinition> JobDefinitions { get; set; }
        public DbSet<User> Users { get; set; }
    }
    
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> 
    { 
        public ApplicationDbContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite($"Data Source=PieWorker.db");
            return new ApplicationDbContext(builder.Options); 
        } 
    }
}