using System.Threading.Tasks;
using PieJobs.Data;
using Microsoft.EntityFrameworkCore;

namespace PieJobs.Services
{
    public interface IUsersService
    {
        public Task<LoginResultDto> Login(string userName, string password);
        public Task SetPassword(string userName, string password);
    }

    public class LoginResultDto
    {
        public bool IsSuccessful { get; set; }
        public string? ApiToken { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    public class UsersService : IUsersService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IPasswordService _passwordService;

        public UsersService(IContextFactory contextFactory, IPasswordService passwordService)
        {
            _contextFactory = contextFactory;
            _passwordService = passwordService;
        }

        public async Task<LoginResultDto> Login(string userName, string password)
        {
            await using var db = _contextFactory.Create();

            var user = await db.Users.SingleOrDefaultAsync(x => x.Name == userName);
            if (user == null)
            {
                return new LoginResultDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "User doesn't exist"
                };
            }
            
            var isValid = _passwordService.IsValid(password, password);
            if (!isValid)
            {
                return new LoginResultDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid password"
                };
            }
            
            return new LoginResultDto
            {
                IsSuccessful = true,
                ApiToken = user.ApiToken
            };
        }

        public async Task SetPassword(string userName, string password)
        {
            await using var db = _contextFactory.Create();

            var user = await db.Users.SingleAsync(x => x.Name == userName);
            var hashedPassword = _passwordService.HashPassword(password);
            user.Password = hashedPassword;
            
            await db.SaveChangesAsync();
        }
    }
}