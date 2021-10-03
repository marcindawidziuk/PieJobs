using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PieJobs.Services;

namespace PieJobs.Api.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;
        
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("login")]
        public Task<LoginResultDto> Login([FromBody] LoginRequestDto dto)
        {
            return _usersService.Login(dto.UserName, dto.Password);
        }

        [Authorize]
        [HttpPost("set-password")]
        public Task SetPassword([FromBody] string password)
        {
            return _usersService.SetPassword(CurrentUserId(), password);
        }

        [Authorize]
        [HttpGet("get-details")]
        public Task<UserDetailsDto> GetDetails()
        {
            return _usersService.GetDetails(CurrentUserId());
        }
    }

    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}