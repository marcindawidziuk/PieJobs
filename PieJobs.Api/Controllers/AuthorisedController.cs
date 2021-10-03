using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PieJobs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId()
        {
            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(user ?? throw new InvalidOperationException("User not authorised"));
        }
    }
}