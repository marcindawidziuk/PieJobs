using System.Linq;
using System.Threading.Tasks;
using PieJobs.Data;
using Microsoft.EntityFrameworkCore;

namespace PieJobs.Services
{
    public interface IUsersService
    {
        public Task<LoginResultDto> Login(string userName, string password);
        public Task SetPassword(int userId, string password);
        public Task<UserDetailsDto> GetDetails(int userId);
    }

    public class UserDetailsDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
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

            var user = await db.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                return new LoginResultDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "User doesn't exist"
                };
            }
            
            var isValid = _passwordService.IsValid(password, user.Password);
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

        public async Task SetPassword(int userId, string password)
        {
            await using var db = _contextFactory.Create();

            var user = await db.Users.SingleAsync(x => x.Id == userId);
            var hashedPassword = _passwordService.HashPassword(password);
            user.Password = hashedPassword;
            
            await db.SaveChangesAsync();
        }

        public async Task<UserDetailsDto> GetDetails(int userId)
        {
            await using var db = _contextFactory.Create();

            return await db.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserDetailsDto
                {
                    UserName = x.UserName,
                    DisplayName = x.DisplayName
                }).SingleAsync();

        }
    }
}