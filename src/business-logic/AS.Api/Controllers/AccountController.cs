using AS.Infrastructure.Services;
using AS.Core.Primitives;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AS.Api.Controllers;

public class AccountController : BaseApiController
{
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IIdentityService _identityService;
    public AccountController(SignInManager<IdentityUser> signInManager, IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.signInManager = signInManager;
        _identityService = identityService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesDefaultResponseType(typeof(BaseApiResponse<AccessTokenResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Неправильный логин/пароль!");
        }

        var result = await _identityService.Login(request);

        return Ok(result);
    }

    [HttpPost]
    [ProducesDefaultResponseType(typeof(BaseApiResponse<object>))]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return Ok();
    }

    [HttpGet]
    [ProducesDefaultResponseType(typeof(BaseApiResponse<InfoResponse>))]
    public async Task<IActionResult> Info()
    {
        var result = await _identityService.Info();

        return Ok(result);
    }
}
