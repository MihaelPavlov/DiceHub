using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Helpers;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
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
    IRepository<TenantSetting> tenantSettingsRepository,
    IUserContext userContext,
    ILocalizationService localizer,
    IUserManagementService userManagementService) : IOwnerService
{
    readonly ILogger<OwnerService> logger = logger;
    readonly UserManager<ApplicationUser> userManager = userManager;
    readonly RoleManager<IdentityRole> roleManager = roleManager;
    readonly IRepository<TenantSetting> tenantSettingsRepository = tenantSettingsRepository;
    readonly IUserContext userContext = userContext;
    readonly ILocalizationService localizer = localizer;
    readonly IUserManagementService userManagementService = userManagementService;

    public async Task<OwnerResult> CreateOwner(CreateOwnerRequest request, CancellationToken cancellationToken)
    {
        if (!await this.userManagementService.HasUserAnyMatchingRole(this.userContext.UserId!, Role.SuperAdmin))
            throw new BadRequestException(this.localizer["OnlySuperAdminCreateOwner"]);

        if (!request.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var owner = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());
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
            EmailConfirmed = true
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

        var dbSettings = await this.tenantSettingsRepository.GetByAsyncWithTracking(x => x.Id == 1, cancellationToken);
        if (dbSettings != null)
        {
            dbSettings.ClubName = request.ClubName;
            dbSettings.PhoneNumber = request.ClubPhoneNumber;
            await this.tenantSettingsRepository.SaveChangesAsync(cancellationToken);
        }

        return new OwnerResult
        {
            Email = afterRegister.Email!,
        };
    }

    public async Task CreateOwnerPassword(CreateOwnerPasswordRequest request)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserByEmailNotFound"]);

        var dbSettings = await this.tenantSettingsRepository.GetByAsync(x => x.Id == 1, CancellationToken.None);
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

        var usersInRole = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());

        if (usersInRole == null)
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

        var usersInRole = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());

        if (usersInRole == null)
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
