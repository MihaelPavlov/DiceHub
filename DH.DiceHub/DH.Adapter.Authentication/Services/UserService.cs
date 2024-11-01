﻿using Azure.Core;
using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DH.Adapter.Authentication.Services;

/// <summary>
/// A default implementation of the <see cref="IUserService"/>.
/// </summary>
public class UserService : IUserService
{
    readonly SignInManager<ApplicationUser> signInManager;
    readonly UserManager<ApplicationUser> userManager;
    readonly RoleManager<IdentityRole> roleManager;
    readonly IJwtService jwtService;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IPermissionStringBuilder _permissionStringBuilder;
    readonly SynchronizeUsersChallengesQueue queue;
    readonly IPushNotificationsService pushNotificationsService;
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository;
    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    /// <param name="signInManager"><see cref="SignInManager<T>"/> for managing user sign-in operations.</param>
    /// <param name="userManager"><see cref="UserManager<T>"/> for managing user-related operations.</param>
    /// <param name="jwtService"><see cref="IJwtService"/> for accessing application jwt authentication logic.</param>
    public UserService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IPermissionStringBuilder permissionStringBuilder, SynchronizeUsersChallengesQueue queue, IPushNotificationsService pushNotificationsService, IRepository<UserDeviceToken> userDeviceTokenRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this._permissionStringBuilder = permissionStringBuilder;
        this.queue = queue;
        this.pushNotificationsService = pushNotificationsService;
        this.userDeviceTokenRepository = userDeviceTokenRepository;
    }

    /// <inheritdoc />
    public async Task<TokenResponseModel?> Login(LoginRequest form)
    {
        var user = await this.userManager.FindByEmailAsync(form.Email);
        if (user is null)
            throw new ValidationErrorsException("Email", "Email or Password is invalid!");


        var roles = await this.userManager.GetRolesAsync(user);
        var result = await this.signInManager.PasswordSignInAsync(user, form.Password, form.RememberMe, false);

        if (!result.Succeeded)
            throw new ValidationErrorsException("Email", "Email or Password is invalid!");

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, RoleHelper.GetRoleKeyByName(roles.First()).ToString()),
                new Claim("permissions",_permissionStringBuilder.GetFromCacheOrBuildPermissionsString( RoleHelper.GetRoleKeyByName(roles.First())))
            };
        var tokenString = this.jwtService.GenerateAccessToken(claims);
        var refreshToken = this.jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10);

        await this.userManager.UpdateAsync(user);

        _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return new TokenResponseModel { AccessToken = tokenString, RefreshToken = refreshToken };
    }

    /// <inheritdoc />
    public async Task RegisterUser(UserRegistrationRequest form)
    {
        if (!form.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var existingUserByEmail = await this.userManager.FindByEmailAsync(form.Email);
        if(existingUserByEmail != null)
            throw new ValidationErrorsException("Exist", "Player with that Email, already exist");

        var existingUserByUsername = await this.userManager.FindByNameAsync(form.Username);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", "Player with that Username, already exist");

        var user = new ApplicationUser() { UserName = form.Username, Email = form.Email, EmailConfirmed = true };
        var createUserResult = await userManager.CreateAsync(user, form.Password);
        if (!createUserResult.Succeeded)
            throw new BadRequestException("User registration failed!");

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == "User"))
            throw new BadRequestException("User registration failed!");

        await this.userManager.AddToRoleAsync(user, "User");

        var afterRegister = await this.userManager.FindByEmailAsync(form.Email);
        if (afterRegister is null)
            throw new NotFoundException("User was not created");

        this.queue.AddSynchronizeNewUserJob(afterRegister.Id);

        await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
        {
            DeviceToken = form.DeviceToken,
            LastUpdated = DateTime.UtcNow,
            UserId = afterRegister.Id
        }, CancellationToken.None);
    }

    public async Task RegisterNotification(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user is null)
            throw new ArgumentNullException("User is not found");
        var userDeviceToken = await this.userDeviceTokenRepository.GetByAsync(x => x.UserId == user.Id, CancellationToken.None);
        await this.pushNotificationsService.SendMessageAsync(new RegistrationMessage(email) { DeviceToken = userDeviceToken.DeviceToken });
    }

    public async Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => ids.Contains(x.Id))
            .Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName ?? "username_placeholder",
                ImageUrl = string.Empty
            })
            .ToListAsync(cancellationToken);
    }
}
