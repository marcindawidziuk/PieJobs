using System;
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
        public DbSet<LogLine> LogLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1, 
                ApiToken = Guid.NewGuid().ToString(),
                DisplayName = "Admin",
                UserName = "admin",
                // default Password is 'password'
                Password = "5Mt/I3P5RbZrSXyI/k5FVz+lTL+ffWO+|10000|NbHfoOfFCJLUZCrSGZ/+VvMvNFB258cp"
            });
            base.OnModelCreating(modelBuilder);
        }
    }
    
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> 
    { 
        public ApplicationDbContext CreateDbContext(string[] args) 
        { 
            
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite($"Data Source=PieJobs.db");
            return new ApplicationDbContext(builder.Options); 
        } 
    }
}