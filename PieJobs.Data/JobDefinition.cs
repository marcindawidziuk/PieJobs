using System;

namespace PieJobs.Data
{
    public class JobDefinition
    {
        public JobDefinition()
        {
            LastModifiedUtc = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }
}