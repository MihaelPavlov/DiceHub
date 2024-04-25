using DH.Adapter.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    readonly IUserService userService;
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginForm form)
    {
       var res= userService.Login(form);

        return Ok(res);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        userService.Logout();

        return Ok();
    }
}
