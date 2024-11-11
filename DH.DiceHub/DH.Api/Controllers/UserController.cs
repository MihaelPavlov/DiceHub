using DH.Domain.Adapters.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DH.Domain.Adapters.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DH.Adapter.Authentication.Filters;
using DH.Application.Common.Commands;
using DH.Domain.Adapters.Authentication.Enums;
using Google.Apis.Download;
using MediatR;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Adapters.PushNotifications;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    readonly IConfiguration configuration;
    readonly IUserService userService;
    readonly IJwtService jwtService;
    readonly IMediator mediator;
    readonly IPushNotificationsService pushNotificationsService;

    public UserController(IConfiguration configuration, IJwtService jwtService, IUserService userService, IMediator mediator, IPushNotificationsService pushNotificationsService)
    {
        this.configuration = configuration;
        this.userService = userService;
        this.jwtService = jwtService;
        this.mediator = mediator;
        this.pushNotificationsService = pushNotificationsService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest form)
    {
        var result = await userService.Login(form);
        return this.Ok(result);
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest form)
    {
        await userService.RegisterUser(form);
        return this.Ok();
    }

    [HttpPost("register-notification")]
    public async Task<IActionResult> RegisterNotification([FromBody] RegistrationNotifcation form)
    {
        var userDeviceToken = await userService.GetDeviceTokenByUserEmail(form.Email);
        await this.pushNotificationsService.SendUserNotificationAsync(new RegistrationMessage(form.Email) { DeviceToken = userDeviceToken.DeviceToken });

        return this.Ok();
    }

    //[HttpPost("info")]
    //[Authorize]
    //public IActionResult UserInfo()
    //{
    //    return this.Ok(new { IsAuthenticated = this.User.Identity.IsAuthenticated, Id = this.User.Claims.First(x => x.Type == ClaimTypes.Sid).Value, Role = this.User.Claims.First(x => x.Type == ClaimTypes.Role).Value });
    //}

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(TokenResponseModel tokenApiModel)
    {
        var result = await this.jwtService.RefreshAccessTokenAsync(tokenApiModel);
        return this.Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("info")]
    public IActionResult UserInfo()
    {
        if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var accessToken = authHeader.ToString().Split(' ').Last();
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecrectKey") ?? throw new ArgumentException("JWT_SecretKey was not specified")));
                var fe_url = configuration.GetValue<string>("Front_End_Application_URL")
             ?? throw new ArgumentException("Front_End_Application_URL was not specified");
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = fe_url,
                    ValidAudience = fe_url,
                    IssuerSigningKey = secretKey
                };

                tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims.DistinctBy(x => x.Type).ToDictionary(claim => claim.Type, claim => claim.Value);

                // The token is valid
                return Ok(claims);
            }
            catch (SecurityTokenException)
            {
                // Token validation failed
                return Unauthorized(new { message = "Token validation failed" });
            }
        }

        return BadRequest();
    }

    [HttpPost("save-token")]
    [ActionAuthorize(UserAction.MessagingCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveToken([FromBody] SaveUserDeviceTokenCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
}
