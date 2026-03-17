using Microsoft.AspNetCore.Mvc;

namespace BuildHub.API.Controllers.Base
{
    /// <summary>
    /// Base api controller, every controller should derive form this.
    /// The class provides basic api controller support and automatic url path computation
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController :  ControllerBase
    {
        protected BaseApiController() 
            : base() 
        { 
        }
    }

}
