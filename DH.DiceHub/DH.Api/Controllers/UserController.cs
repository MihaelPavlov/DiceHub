using DH.Domain.Adapters.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DH.Domain.Adapters.Authentication.Models;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    readonly IUserService userService;
    readonly IJwtService jwtService;

    public UserController(IJwtService jwtService, IUserService userService)
    {
        this.userService = userService;
        this.jwtService = jwtService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest form)
    {
        var result = await userService.Login(form);
        return this.Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest form)
    {
        await userService.Register(form);
        return this.Ok();
    }

    [HttpPost("info")]
    [Authorize]
    public IActionResult UserInfo()
    {
        return this.Ok(new { IsAuthenticated = this.User.Identity.IsAuthenticated, Id = this.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value, Role = this.User.Claims.First(x => x.Type == ClaimTypes.Role).Value });
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(TokenResponseModel tokenApiModel)
    {
        var result = await this.jwtService.RefreshAccessTokenAsync(tokenApiModel);
        return this.Ok(result);
    }
}
