using System;

namespace PieJobs.Data
{
    public class Job
    {
        public int Id { get; set; }
        public int? JobDefinitionId { get; set; }
        public virtual JobDefinition JobDefinition { get; set; }
        public DateTime ScheduleDateTimeUtc { get; set; }
        public DateTime? StartedDateTimeUtc { get; set; }
        public DateTime? FinishedDateTimeUtc { get; set; }
        public string Command { get; set; }
        public JobStatus Status { get; set; }
    }
}