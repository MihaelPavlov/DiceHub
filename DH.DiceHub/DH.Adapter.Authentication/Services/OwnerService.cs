using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Helpers;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Services.TenantSettingsService;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Authentication.Services;

internal class OwnerService(
    ILogger<OwnerService> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ITenantService tenantService,
    IRepository<TenantSetting> tenantSettingsRepository,
    IUserContext userContext,
    ILocalizationService localizer,
    IUserManagementService userManagementService,
    ITenantSettingsCacheService tenantSettingsCacheService) : IOwnerService
{
    readonly ILogger<OwnerService> logger = logger;
    readonly UserManager<ApplicationUser> userManager = userManager;
    readonly RoleManager<IdentityRole> roleManager = roleManager;
    readonly ITenantService tenantService = tenantService;
    readonly IRepository<TenantSetting> tenantSettingsRepository = tenantSettingsRepository;
    readonly IUserContext userContext = userContext;
    readonly ILocalizationService localizer = localizer;
    readonly IUserManagementService userManagementService = userManagementService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;

    public async Task<OwnerResult> CreateOwner(CreateOwnerRequest request, CancellationToken cancellationToken)
    {
        if (!await this.userManagementService.HasUserAnyMatchingRole(this.userContext.UserId!, Role.SuperAdmin))
            throw new BadRequestException(this.localizer["OnlySuperAdminCreateOwner"]);

        if (!request.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var tenantId = this.userContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new BadRequestException("TenantId is required.");

        var owner = (await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString()))
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .ToList();
        if (owner.Count > 1)
            throw new ValidationErrorsException("Owner", this.localizer["OwnerAlreadyExists"]);

        var existingUserByEmail = await this.userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Exist", this.localizer["UserExistEmail"]);

        var username = request.Email;
        var existingUserByUsername = await this.userManager.FindByNameAsync(username);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", this.localizer["UserFirstLastNamesExists"]);

        var user = new ApplicationUser()
        {
            UserName = username,
            Email = request.Email,
            PhoneNumber = request.ClubPhoneNumber,
            EmailConfirmed = true,
            TenantId = tenantId
        };
        var generatedRandomPassword = PasswordGenerator.GenerateRandomPassword();
        var createUserResult = await userManager.CreateAsync(user, generatedRandomPassword);
        if (!createUserResult.Succeeded)
            throw new BadRequestException(this.localizer["UserRegistrationFailed"]);

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == Role.Owner.ToString()))
        {
            this.logger.LogCritical("Role {Role} was not found", Role.Owner.ToString());
            throw new BadRequestException(this.localizer["UserRegistrationFailedDuringRoleAssignment"]);
        }

        await this.userManager.AddToRoleAsync(user, Role.Owner.ToString());

        var afterRegister = await this.userManager.FindByEmailAsync(request.Email);
        if (afterRegister is null)
            throw new NotFoundException(this.localizer["UserNotCreated"]);

        var tenant = await this.tenantService.GetCurrentTenantAsync(cancellationToken);
        if (tenant.TenantSetting != null)
        {
            var dbSettings = await this.tenantSettingsRepository.GetByAsyncWithTracking(x => x.Id == tenant.TenantSettingId, cancellationToken);
            if (dbSettings != null)
            {
                dbSettings.ClubName = request.ClubName;
                dbSettings.PhoneNumber = request.ClubPhoneNumber;
                await this.tenantSettingsRepository.SaveChangesAsync(cancellationToken);
            }
            this.tenantSettingsCacheService.Clear(tenant.Id);
        }

        return new OwnerResult
        {
            Email = afterRegister.Email!,
        };
    }

    public async Task<CreateOwnerForTenantSetupResult> CreateOwnerForTenantSetup(
        CreateOwnerForTenantSetupRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenantId))
            throw new BadRequestException("TenantId is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationErrorsException("Email", this.localizer["OwnerValidationEmailRequired"]);

        var existingUserByEmail = await this.userManager.Users
            .Where(x => x.Email == request.Email && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Email", this.localizer["UserExistEmail"]);

        var username = request.Email.Trim();
        var existingUserByUsername = await this.userManager.Users
            .Where(x => x.UserName == username && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Username", this.localizer["UserExistUsername"]);

        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
        {
            this.logger.LogCritical("Role {Role} was not found", Role.Owner.ToString());
            throw new BadRequestException(this.localizer["UserRegistrationFailedDuringRoleAssignment"]);
        }

        var generatedRandomPassword = PasswordGenerator.GenerateRandomPassword();
        var user = new ApplicationUser
        {
            UserName = username,
            Email = request.Email.Trim(),
            PhoneNumber = request.ClubPhoneNumber,
            EmailConfirmed = true,
            TenantId = request.TenantId,
        };

        var createUserResult = await this.userManager.CreateAsync(user, generatedRandomPassword);
        if (!createUserResult.Succeeded)
            throw new ValidationErrorsException("General", this.localizer["UserRegistrationFailed"]);

        await this.userManager.AddToRoleAsync(user, Role.Owner.ToString());

        return new CreateOwnerForTenantSetupResult
        {
            UserId = user.Id,
            Email = user.Email!,
            TemporaryPassword = generatedRandomPassword,
        };
    }

    public async Task CreateOwnerPassword(CreateOwnerPasswordRequest request)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserByEmailNotFound"]);

        var dbSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(CancellationToken.None);
        if (dbSettings != null)
        {
            if (!request.ClubPhoneNumber.Equals(dbSettings.PhoneNumber))
                throw new ValidationErrorsException(
                    "ClubPhoneNumber",
                    this.localizer["PhoneNumberMismatch"]);
        }

        if (!request.NewPassword.Equals(request.ConfirmPassword))
            throw new ValidationErrorsException("Password", this.localizer["PasswordMismatch"]);

        var result = await this.userManager.ResetPasswordAsync(user!, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var isTokenInvalid = result.Errors.Select(x => x.Description).Any(x => x.Contains("Invalid token"));

            if (isTokenInvalid)
            {
                throw new ValidationErrorsException(
                    "InvalidToken",
                    this.localizer["PasswordResetLinkInvalidOrExpired"]);
            }

            this.logger.LogError(
               "CreateOwnerPassword failed for user with email: {Email}. Errors: {Errors}",
               request.Email,
               string.Join("; ", result.Errors.Select(x => x.Description))
           );

            throw new ValidationErrorsException(
                "Password",
                this.localizer["PasswordSetError"]);
        }
    }

    public async Task<OwnerResult?> GetOwner(CancellationToken cancellationToken)
    {
        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
        {
            this.logger.LogCritical("Owner role was not found during get owner operation.");
            throw new InfrastructureException("Role Owner does not exist.");
        }

        var usersInRole = (await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString()))
            .Where(x => x.TenantId == this.userContext.TenantId && !x.IsDeleted)
            .ToList();

        if (usersInRole.Count == 0)
            return null;

        if (usersInRole.Count() > 1)
        {
            this.logger.LogCritical("More then one user with role Owner were found");
            throw new InfrastructureException("More than one owner was found. This violates the system constraints");
        }

        return new OwnerResult
        {
            Email = usersInRole.First().Email!
        };
    }

    public async Task DeleteOwner(CancellationToken cancellationToken)
    {
        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
        {
            this.logger.LogCritical("Owner roles was not found during deleting owner operation");
            throw new InfrastructureException("Role Owner does not exist.");
        }

        var usersInRole = (await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString()))
            .Where(x => x.TenantId == this.userContext.TenantId && !x.IsDeleted)
            .ToList();

        if (usersInRole.Count == 0)
        {
            this.logger.LogCritical("Owner for deletion was not found.");
            throw new InfrastructureException("Owner for deletion was not found.");
        }

        if (usersInRole.Count() > 1)
        {
            this.logger.LogCritical("More then one user with role Owner were found");
            throw new InfrastructureException("More than one owner was found. This violates the system constraints");
        }

        await this.userManager.DeleteAsync(usersInRole.First());
    }
}
