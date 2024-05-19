using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    /// <param name="signInManager"><see cref="SignInManager<T>"/> for managing user sign-in operations.</param>
    /// <param name="userManager"><see cref="UserManager<T>"/> for managing user-related operations.</param>
    /// <param name="jwtService"><see cref="IJwtService"/> for accessing application jwt authentication logic.</param>
    public UserService(SignInManager<ApplicationUser> signInManager, IJwtService jwtService, UserManager<ApplicationUser> userManager)
    {
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<TokenResponseModel> Login(LoginRequest form)
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
                new Claim(ClaimTypes.Role, roles.First())
            };

            var tokenString = this.jwtService.GenerateAccessToken(claims);
            var refreshToken = this.jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await userManager.UpdateAsync(user);

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
        await userManager.AddToRoleAsync(user, "User");
    }
}
