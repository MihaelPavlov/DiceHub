using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
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
    readonly IJwtService jwtService;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IPermissionStringBuilder _permissionStringBuilder;
    readonly SynchronizeUsersChallengesQueue queue;

    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    /// <param name="signInManager"><see cref="SignInManager<T>"/> for managing user sign-in operations.</param>
    /// <param name="userManager"><see cref="UserManager<T>"/> for managing user-related operations.</param>
    /// <param name="jwtService"><see cref="IJwtService"/> for accessing application jwt authentication logic.</param>
    public UserService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
        UserManager<ApplicationUser> userManager, IPermissionStringBuilder permissionStringBuilder, SynchronizeUsersChallengesQueue queue)
    {
        _httpContextAccessor = httpContextAccessor;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.userManager = userManager;
        this._permissionStringBuilder = permissionStringBuilder;
        this.queue = queue;
    }

    /// <inheritdoc />
    public async Task<TokenResponseModel> Login(LoginRequest form, bool fromRegister = false)
    {
        var user = await this.userManager.FindByEmailAsync(form.Email);
        if (user is null)
            throw new ArgumentNullException("User is not found");
        var roles = await this.userManager.GetRolesAsync(user);
        var result = await this.signInManager.PasswordSignInAsync(user, form.Password, true, false);

        if (result.Succeeded)
        {
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
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await this.userManager.UpdateAsync(user);

            _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            if (fromRegister)
            {
                this.queue.AddSynchronizeNewUserJob(user.Id);
            }
            return new TokenResponseModel { AccessToken = tokenString, RefreshToken = refreshToken };
        }
        return new TokenResponseModel { AccessToken = null, RefreshToken = null };
    }

    /// <inheritdoc />
    public async Task Register(RegistrationRequest form)
    {
        var user = new ApplicationUser() { UserName = form.Username, Email = form.Email, EmailConfirmed = true };
        var createUserResult = await userManager.CreateAsync(user, form.Password);
        if (!createUserResult.Succeeded)
        {
            throw new BadHttpRequestException("User registration failed!");
        }

        //TODO: Update the logic 
        await this.userManager.AddToRoleAsync(user, "User");
        await Login(new LoginRequest { Email = form.Email, Password = form.Password }, true);
        //TODO: _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
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
