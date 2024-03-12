using AS.Core.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.Identity.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected new IActionResult Ok(object? value = null)
        {
            var response = new BaseApiResponse<object>(value);

            return base.Ok(response);
        }

        protected new IActionResult BadRequest(string message)
        {
            var response = new BaseApiResponse<object>(message);
            return base.BadRequest(response);
        }
    }
}
