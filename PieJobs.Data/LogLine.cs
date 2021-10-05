using System;

namespace PieJobs.Data
{
    public class LogLine
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public bool IsError { get; set; } 
        public int LineNumber { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public string Text { get; set; }
    }
}