﻿using DH.Domain.Adapters.Authentication.Services;
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
using DH.Application.Stats.Queries;
using DH.Application.Emails.Commands;
using DH.Domain.Adapters.Email.Models;
using DH.Domain.Entities;
using DH.Domain.Adapters.Email;
using DH.Domain.Models.Common;

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
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest form, CancellationToken cancellationToken)
    {
        var response = await userService.RegisterUser(form);

        var isEmailSendedSuccessfully = false;
        if (response.IsRegistrationSuccessfully)
            isEmailSendedSuccessfully = await this.mediator.Send(new SendRegistrationEmailConfirmationCommand(ByUserId: response.UserId, null), cancellationToken);

        response.IsEmailConfirmationSendedSuccessfully = isEmailSendedSuccessfully;
        return this.Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("send-email-confirmation-request/{email}")]
    public async Task<IActionResult> SendEmailConfirmationRequest(string email, CancellationToken cancellationToken)
    {
        var isSuccessfully = await this.mediator.Send(new SendRegistrationEmailConfirmationCommand(null, ByEmail: email), cancellationToken);
        return this.Ok(isSuccessfully);
    }

    [AllowAnonymous]
    [HttpPost("send-employee-password-reset-request/{email}")]
    public async Task<IActionResult> SendEmployeePasswordResetRequest(string email, CancellationToken cancellationToken)
    {
        var isSuccessfully = await this.mediator.Send(new SendEmployeeCreatePasswordEmailCommand(email), cancellationToken);
        return this.Ok(isSuccessfully);
    }

    [AllowAnonymous]
    [HttpPost("send-owner-password-reset-request/{email}")]
    public async Task<IActionResult> SendOwnerPasswordResetRequest(string email, CancellationToken cancellationToken)
    {
        var isSuccessfully = await this.mediator.Send(new SendOwnerCreatePasswordEmailCommand(email), cancellationToken);
        return this.Ok(isSuccessfully);
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await this.userService.ConfirmEmail(request.Email, request.Token, cancellationToken);
        return this.Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password/{email}")]
    public async Task<IActionResult> ForgotPassword(string email, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new SendForgotPasswordEmailCommand(email), cancellationToken);
        return this.Ok();
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await this.userService.ResetPassword(request);
        return this.Ok();
    }

    [AllowAnonymous]
    [HttpPost("create-employee-password")]
    public async Task<IActionResult> CreateEmployeePassword([FromBody] CreateEmployeePasswordRequest request)
    {
        await this.userService.CreateEmployeePassword(request);
        return this.Ok();
    }

    [AllowAnonymous]
    [HttpPost("create-owner-password")]
    public async Task<IActionResult> CreateOwnerPassword([FromBody] CreateOwnerPasswordRequest request)
    {
        await this.userService.CreateOwnerPassword(request);
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
        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Ok(null);

        var accessToken = authHeader.ToString().Split(' ').Last();
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(accessToken))
            return Ok(null);

        try
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["TokenIssuer"],
                ValidAudiences = configuration.GetSection("APIs_Audience_URLs").Get<string[]>(),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWT_SecretKey"]!))
            };

            tokenHandler.ValidateToken(accessToken, validationParams, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims
                .DistinctBy(c => c.Type)
                .ToDictionary(c => c.Type, c => c.Value);

            return Ok(claims);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { message = "Token validation failed" });
        }
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
        var employees = await this.userService.GetUserListByRoles([Role.User, Role.Staff, Role.Owner], cancellationToken);
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
    [HttpGet("get-employee-by-id/{id}")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserModel>))]
    public async Task<IActionResult> GetEmployeeById(string id, CancellationToken cancellationToken)
    {
        var employee = await this.userService.GetUserById(id, cancellationToken);
        return Ok(employee);
    }

    [Authorize]
    [HttpPost("create-employee")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employeeResult = await this.userService.CreateEmployee(request, cancellationToken);
        await this.mediator.Send(new SendEmployeeCreatePasswordEmailCommand(
            employeeResult.Email), cancellationToken);

        await this.mediator.Send(new UpdateUserSettingsCommand(new UserSettingsDto
        {
            PhoneNumber = request.PhoneNumber,
        }, employeeResult.UserId), cancellationToken);

        return Ok();
    }

    [Authorize]
    [HttpPut("update-employee")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employeeResult = await this.userService.UpdateEmployee(request, cancellationToken);
        if (employeeResult.IsEmailChanged)
        {
            await this.mediator.Send(new SendEmployeeCreatePasswordEmailCommand(
                employeeResult.Email), cancellationToken);
        }

        await this.mediator.Send(new UpdateUserSettingsCommand(new UserSettingsDto
        {
            PhoneNumber = request.PhoneNumber,
        }, employeeResult.UserId), cancellationToken);

        return Ok();
    }

    [Authorize]
    [HttpPost("delete-employee")]
    [ActionAuthorize(UserAction.EmployeesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEmployee(string employeeId, CancellationToken cancellationToken)
    {
        await this.userService.DeleteEmployee(employeeId);
        //TODO: ON Delete employee we should delete the TenantUserSettings for the deleted user
        return Ok();
    }

    [Authorize]
    [HttpGet("get-owner")]
    [ActionAuthorize(UserAction.OwnerCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OwnerResult))]
    public async Task<IActionResult> GetOwner(CancellationToken cancellationToken)
    {
        var owner = await this.userService.GetOwner(cancellationToken);
        return Ok(owner);
    }

    [Authorize]
    [HttpPost("create-owner")]
    [ActionAuthorize(UserAction.OwnerCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOwner([FromBody] CreateOwnerRequest request, CancellationToken cancellationToken)
    {
        var ownerResult = await this.userService.CreateOwner(request, cancellationToken);

        await this.mediator.Send(new SendOwnerCreatePasswordEmailCommand(
            ownerResult.Email), cancellationToken);

        return Ok();
    }

    [Authorize]
    [HttpDelete("delete-owner")]
    [ActionAuthorize(UserAction.OwnerCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOwner(CancellationToken cancellationToken)
    {
        await this.userService.DeleteOwner(cancellationToken);

        return Ok();
    }

    [Authorize]
    [HttpGet("get-user-stats")]
    [ActionAuthorize(UserAction.UsersRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserStatsQueryModel))]
    public async Task<IActionResult> GetUserStats(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserStatsQuery(), cancellationToken);
        return Ok(result);
    }
}
