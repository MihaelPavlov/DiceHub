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
using MediatR;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.Authentication.Models.Enums;

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

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest form)
    {
        var result = await userService.Login(form);
        return this.Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest form)
    {
        await userService.RegisterUser(form);
        return this.Ok();
    }

    [AllowAnonymous]
    [HttpPost("register-notification")]
    public async Task<IActionResult> RegisterNotification([FromBody] RegistrationNotifcation form)
    {
        var userDeviceToken = await userService.GetDeviceTokenByUserEmail(form.Email);
        await this.pushNotificationsService.SendUserNotificationAsync(new RegistrationMessage(form.Email) { DeviceToken = userDeviceToken.DeviceToken });

        return this.Ok();
    }

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
                var apiAudiences = configuration.GetSection("APIs_Audience_URLs").Get<string[]>()
                    ?? throw new ArgumentException("APIs_Audience_URLs was not specified");

                var issuer = configuration.GetValue<string>("TokenIssuer")
                    ?? throw new ArgumentException("TokenIssuer was not specified");

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SecretKey")
                    ?? throw new ArgumentException("JWT_SecretKey was not specified")));

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudiences = apiAudiences,
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

    [Authorize]
    [HttpPost("save-token")]
    [ActionAuthorize(UserAction.MessagingCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveToken([FromBody] SaveUserDeviceTokenCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet("get-user-list")]
    [ActionAuthorize(UserAction.UsersRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserByRoleModel>))]
    public async Task<IActionResult> GetUserList(CancellationToken cancellationToken)
    {
        var employees = await this.userService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);
        return Ok(employees);
    }

    [Authorize]
    [HttpGet("get-employee-list")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserByRoleModel>))]
    public async Task<IActionResult> GetEmployeeList(CancellationToken cancellationToken)
    {
        var employees = await this.userService.GetUserListByRole(Role.Staff, cancellationToken);
        return Ok(employees);
    }

    [Authorize]
    [HttpPost("create-employee")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        await this.userService.CreateEmployee(request, cancellationToken);
        return Ok();
    }
}
