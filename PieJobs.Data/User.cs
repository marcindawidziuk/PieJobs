using System;

namespace PieJobs.Data
{
    public class User
    {
        public User()
        {
            ApiToken = Guid.NewGuid().ToString();
        }
        
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApiToken { get; set; }
    }
}