using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Authentication.Services;

internal class AuthenticationService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    ISystemUserContextAccessor userContextAccessor,
    ILocalizationService localizer,
    IRepository<UserDeviceToken> userDeviceTokenRepository) : IAuthenticationService
{
    readonly SignInManager<ApplicationUser> signInManager = signInManager;
    readonly UserManager<ApplicationUser> userManager = userManager;
    readonly ISystemUserContextAccessor userContextAccessor = userContextAccessor;
    readonly ILocalizationService localizer = localizer;
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository = userDeviceTokenRepository;
    readonly ITokenService tokenService = tokenService;

    public async Task<TokenResponseModel?> Login(LoginRequest form)
    {
        var user = await GetUserByEmailAsync(form.Email, "InvalidEmailOrPass");

        if (!await userManager.IsEmailConfirmedAsync(user!))
            throw new ValidationErrorsException("EmailNotConfirmed", this.localizer["EmailNotConfirmed"]);

        if (!await userManager.CheckPasswordAsync(user!, form.Password))
            throw new ValidationErrorsException("Email", localizer["InvalidEmailOrPass"]);

        if (!string.IsNullOrEmpty(form.TimeZone) && form.TimeZone != user!.TimeZone)
        {
            user.TimeZone = form.TimeZone!;
            await userManager.UpdateAsync(user);
        }

        await UpdateDeviceTokenAsync(user, form.DeviceToken);

        return await IssueUserTokensAsync(user!);
    }

    public async Task<TokenResponseModel?> ConfirmEmail(string email, string token, CancellationToken cancellationToken)
    {
        var user = await GetUserByEmailAsync(email, "UserByEmailNotFound");

        var result = await this.userManager.ConfirmEmailAsync(user!, token);

        if (result.Succeeded)
        {
            await this.signInManager.SignInAsync(user!, true);

            return await IssueUserTokensAsync(user!);
        }

        throw new ValidationErrorsException("InvalidToken", this.localizer["ConfirmEmailInvalidToken"]);
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await GetUserByEmailAsync(request.Email, "UserNotFound");

        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            throw new ValidationErrorsException("Password", this.localizer["PasswordMismatch"]);

        var result = await this.userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
            throw new ValidationErrorsException("Password", this.localizer["PasswordResetFailed"]);
    }

    public async Task<bool> Logout(string userId, string tenantId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null || user.TenantId != tenantId)
            return false;

        // Invalidate refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        // Optional: kill all sessions
        user.SecurityStamp = Guid.NewGuid().ToString();

        await userManager.UpdateAsync(user);

        return true;
    }

    private async Task<TokenResponseModel?> IssueUserTokensAsync(ApplicationUser user)
    {
        var claims = await this.tokenService.BuildUserClaimsAsync(user.Id);

        var accessToken = this.tokenService.GenerateAccessToken(claims);
        var refreshToken = this.tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = this.tokenService.GetRefreshTokenExpiryTime();

        this.userContextAccessor.Set(
            new UserContext(user.TenantId, user.Id, null, null, null));
        await this.userManager.UpdateAsync(user);

        return new TokenResponseModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            TenantId = user.TenantId
        };
    }

    private async Task UpdateDeviceTokenAsync(ApplicationUser user, string? deviceToken)
    {
        this.userContextAccessor.Set(
            new UserContext(user.TenantId, user.Id, null, null, null));

        var userDeviceToken = await this.userDeviceTokenRepository.GetByAsyncWithTracking(x => x.UserId == user!.Id, CancellationToken.None);
        if (userDeviceToken is null && deviceToken is not null)
        {
            await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
            {
                DeviceToken = deviceToken,
                LastUpdated = DateTime.UtcNow,
                UserId = user!.Id
            }, CancellationToken.None);
        }
        else if (userDeviceToken is not null && !string.IsNullOrEmpty(deviceToken))
        {
            userDeviceToken.DeviceToken = deviceToken;
            await this.userDeviceTokenRepository.SaveChangesAsync(CancellationToken.None);
        }
    }

    private async Task<ApplicationUser> GetUserByEmailAsync(string email, string errorKey)
    {
        var user = await this.userManager.Users
            .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);

        if (user == null)
            throw new NotFoundException(this.localizer[errorKey]);

        return user;
    }
}
