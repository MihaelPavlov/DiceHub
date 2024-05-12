using DH.Adapter.Authentication.Services;
using DH.Domain.Adapters.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    readonly IUserService userService;
    readonly IUserContext context;
    public UserController(IUserService userService, IUserContext context)
    {
        this.userService = userService;
        this.context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginForm form)
    {
        var result = await userService.Login(form);
        return this.Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterForm form)
    {
        await userService.Register(form);
        return this.Ok();
    }

    [HttpPost("info")]
    [Authorize]
    public IActionResult UserInfo()
    {
        return this.Ok(new { IsAuthenticated = this.User.Identity.IsAuthenticated, this.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value });
    }
}
