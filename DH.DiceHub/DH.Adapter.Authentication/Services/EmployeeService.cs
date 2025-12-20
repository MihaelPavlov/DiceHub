using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Helpers;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Authentication.Services;

internal class EmployeeService(
    ILogger<EmployeeService> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IUserContext userContext,
    IUserManagementService userManagementService,
    ILocalizationService localizer) : IEmployeeService
{
    readonly ILogger<EmployeeService> logger = logger;
    readonly UserManager<ApplicationUser> userManager = userManager;
    readonly RoleManager<IdentityRole> roleManager = roleManager;
    readonly IUserContext userContext = userContext;
    readonly IUserManagementService userManagementService = userManagementService;
    readonly ILocalizationService localizer = localizer;

    public async Task<EmployeeResult> CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await this.userManagementService.HasUserAnyMatchingRole(this.userContext.UserId!, Role.SuperAdmin, Role.Owner))
            throw new BadRequestException(this.localizer["OnlyOwnerCreateEmployees"]);

        if (!request.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var existingUserByEmail = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Email", this.localizer["UserExistEmail"]);

        var username = $"{request.FirstName}, {request.LastName}";
        var existingUserByUsername = await this.userManager.Users
           .Where(x => x.UserName == username && !x.IsDeleted).FirstOrDefaultAsync();

        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", this.localizer["UserFirstLastNamesExists"]);

        var user = new ApplicationUser()
        {
            UserName = username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true
        };
        var generatedRandomPassword = PasswordGenerator.GenerateRandomPassword();
        var createUserResult = await userManager.CreateAsync(user, generatedRandomPassword);
        if (!createUserResult.Succeeded)
            throw new BadRequestException(this.localizer["UserRegistrationFailed"]);

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == Role.Staff.ToString()))
        {
            this.logger.LogCritical("Role {Role} was not found", Role.Staff.ToString());
            throw new BadRequestException(this.localizer["UserRegistrationFailedDuringRoleAssignment"]);
        }

        await this.userManager.AddToRoleAsync(user, Role.Staff.ToString());

        var afterRegister = await this.userManager.FindByEmailAsync(request.Email);
        if (afterRegister is null)
            throw new NotFoundException(this.localizer["UserNotCreated"]);

        return new EmployeeResult
        {
            UserId = afterRegister.Id,
            Email = afterRegister.Email!,
        };
    }

    public async Task<EmployeeResult> UpdateEmployee(UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await this.userManagementService.HasUserAnyMatchingRole(this.userContext.UserId!, Role.SuperAdmin, Role.Owner))
            throw new BadRequestException(this.localizer["OnlyOwnerCanUpdateEmployee"]);

        if (!request.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var existingUser = await this.userManager.Users
           .Where(x => x.Id == request.Id && !x.IsDeleted).FirstOrDefaultAsync();

        if (existingUser is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        var isEmailChanged = false;
        var oldEmail = string.Empty;
        if (existingUser.Email != request.Email)
        {
            var existingUserByEmail = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();

            if (existingUserByEmail != null)
                throw new ValidationErrorsException("Exist", this.localizer["UserExistEmail"]);

            isEmailChanged = true;
            oldEmail = existingUser.Email;
            existingUser.Email = request.Email;
        }

        var newUsername = $"{request.FirstName}, {request.LastName}";

        if (newUsername != existingUser.UserName)
        {
            var existingUserByUsername = await this.userManager.Users
                .Where(x => x.UserName == newUsername && !x.IsDeleted).FirstOrDefaultAsync();

            if (existingUserByUsername != null)
                throw new ValidationErrorsException("Exist", this.localizer["UserFirstLastNamesExists"]);

            existingUser.UserName = newUsername;
        }

        existingUser.PhoneNumber = request.PhoneNumber;

        var updatedUserResult = await userManager.UpdateAsync(existingUser);
        if (!updatedUserResult.Succeeded)
            throw new BadRequestException(this.localizer["UserRegistrationFailed"]);

        return new EmployeeResult
        {
            UserId = existingUser.Id,
            Email = existingUser.Email!,
            IsEmailChanged = isEmailChanged,
        };
    }

    public async Task CreateEmployeePassword(CreateEmployeePasswordRequest request)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserByEmailNotFound"]);

        if (!request.PhoneNumber.Equals(user.PhoneNumber))
            throw new ValidationErrorsException("PhoneNumber", this.localizer["PhoneNumberMismatch"]);

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
                "CreateEmployeePassword failed for user with email: {Email}. Errors: {Errors}",
                request.Email,
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
            throw new ValidationErrorsException("Password", this.localizer["PasswordSetError"]);
        }
    }

    public async Task<EmployeeModel?> GetEmployeeId(string id, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new EmployeeModel
            {
                Id = x.Id,
                UserName = x.UserName ?? string.Empty,
                Email = x.Email ?? string.Empty,
                PhoneNumber = x.PhoneNumber ?? string.Empty,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            user.FirstName = user.UserName.Split(", ").FirstOrDefault() ?? string.Empty;
            user.LastName = user.UserName.Split(", ").LastOrDefault() ?? string.Empty;
        }

        return user;
    }

    public async Task DeleteEmployee(string employeeId)
    {
        var user = await this.userManager.Users
           .Where(x => x.Id == employeeId && !x.IsDeleted).FirstOrDefaultAsync()
                ?? throw new BadRequestException(this.localizer["EmployeeDeletionFailed"]);

        var userId = user.Id.Replace("-", "");

        user.IsDeleted = true;
        user.UserName = $"{userId},deleted,{user.UserName}";
        user.Email = $"{userId},deleted,{user.Email}";

        var res = await this.userManager.UpdateAsync(user);
    }
}
