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
        // The single login field accepts either the email or the username. A miss on
        // both is a credential validation failure (422), not a 404 - the UI only knows
        // how to render the former (error.errors.*).
        var user = await FindActiveUserByEmailOrUserNameAsync(form.Email)
            ?? throw new ValidationErrorsException("Email", this.localizer["InvalidEmailOrPass"]);

        if (!await userManager.IsEmailConfirmedAsync(user!))
            throw new ValidationErrorsException("EmailNotConfirmed", this.localizer["EmailNotConfirmed"]);

        if (!await userManager.CheckPasswordAsync(user!, form.Password))
            throw new ValidationErrorsException("Email", localizer["InvalidEmailOrPass"]);

        var roles = await this.userManager.GetRolesAsync(user);
        var isSuperAdmin = roles.Contains("SuperAdmin");
        var effectiveTenantId = isSuperAdmin
            ? !string.IsNullOrWhiteSpace(form.TenantId) ? form.TenantId : "system"
            : user.TenantId;

        if (string.IsNullOrWhiteSpace(effectiveTenantId))
            throw new ValidationErrorsException("TenantId", "TenantId is required.");

        if (!isSuperAdmin && effectiveTenantId != form.TenantId)
            throw new ValidationErrorsException("TenantId", "Tenant mismatch.");

        this.userContextAccessor.Set(
            new UserContext(effectiveTenantId, user.Id, null, null, null));

        if (!string.IsNullOrEmpty(form.TimeZone) && form.TimeZone != user!.TimeZone)
        {
            user.TimeZone = form.TimeZone!;
            await userManager.UpdateAsync(user);
        }

        await UpdateDeviceTokenAsync(user, form.DeviceToken, effectiveTenantId);

        return await IssueUserTokensAsync(user!, effectiveTenantId);
    }

    public async Task<TokenResponseModel?> ConfirmEmail(string email, string token, CancellationToken cancellationToken)
    {
        var user = await GetUserByEmailAsync(email, "UserByEmailNotFound");

        var result = await this.userManager.ConfirmEmailAsync(user!, token);

        if (result.Succeeded)
        {
            await this.signInManager.SignInAsync(user!, true);

            return await IssueUserTokensAsync(user!, user!.TenantId);
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

    public async Task ChangePassword(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ValidationErrorsException("User", this.localizer["UserNotFound"]);

        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            throw new ValidationErrorsException("ConfirmPassword", this.localizer["PasswordMismatch"]);

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var isCurrentPasswordWrong = result.Errors.Any(x => x.Code == "PasswordMismatch");
            throw new ValidationErrorsException(
                "CurrentPassword",
                isCurrentPasswordWrong
                    ? this.localizer["CurrentPasswordIncorrect"]
                    : (result.Errors.FirstOrDefault()?.Description ?? this.localizer["PasswordResetFailed"]));
        }
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

    private async Task<TokenResponseModel?> IssueUserTokensAsync(ApplicationUser user, string tenantId)
    {
        var claims = await this.tokenService.BuildUserClaimsAsync(user.Id, tenantId);

        var accessToken = this.tokenService.GenerateAccessToken(claims);
        var refreshToken = this.tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = this.tokenService.GetRefreshTokenExpiryTime();

        this.userContextAccessor.Set(
            new UserContext(tenantId, user.Id, null, null, null));
        await this.userManager.UpdateAsync(user);

        return new TokenResponseModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            TenantId = tenantId
        };
    }

    private async Task UpdateDeviceTokenAsync(ApplicationUser user, string? deviceToken, string tenantId)
    {
        this.userContextAccessor.Set(
            new UserContext(tenantId, user.Id, null, null, null));

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
        return await FindActiveUserByEmailAsync(email)
            ?? throw new NotFoundException(this.localizer[errorKey]);
    }

    /// <summary>
    /// Looks up a non-deleted user by email. Matches on <see cref="ApplicationUser.NormalizedEmail"/>
    /// (as <see cref="UserManager{T}.FindByEmailAsync"/> does) so a differently-cased address -
    /// e.g. a mobile keyboard capitalising the first letter on the login screen but not at
    /// registration - still resolves to the same account instead of looking unknown.
    /// </summary>
    private async Task<ApplicationUser?> FindActiveUserByEmailAsync(string email)
    {
        var normalizedEmail = this.userManager.NormalizeEmail(email);

        return await this.userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail && !x.IsDeleted);
    }

    /// <summary>
    /// Login lookup: the single credential field accepts the email OR the username
    /// (registration collects both). Matches on the normalized forms, case-insensitively,
    /// the same way <see cref="UserManager{T}.FindByEmailAsync"/> / <c>FindByNameAsync</c> do.
    /// </summary>
    private async Task<ApplicationUser?> FindActiveUserByEmailOrUserNameAsync(string emailOrUserName)
    {
        var normalizedEmail = this.userManager.NormalizeEmail(emailOrUserName);
        var normalizedUserName = this.userManager.NormalizeName(emailOrUserName);

        return await this.userManager.Users
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted
                && (x.NormalizedEmail == normalizedEmail || x.NormalizedUserName == normalizedUserName));
    }
}
