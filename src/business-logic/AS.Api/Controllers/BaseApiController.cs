using AS.Core.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AS.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public abstract class BaseApiController(IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        protected Guid UserId = Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? 
            userId : 
            throw new UnauthorizedAccessException("Unauthorized user");

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
