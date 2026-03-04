using Microsoft.AspNetCore.Mvc;

namespace BuildHub.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("live")]
        public StatusCodeResult GetIsSystemLive() => Ok();
    }
}
