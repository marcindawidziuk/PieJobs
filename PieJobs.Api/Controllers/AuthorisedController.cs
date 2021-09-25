using Microsoft.AspNetCore.Mvc;

namespace PieJobs.Api.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase { }
}