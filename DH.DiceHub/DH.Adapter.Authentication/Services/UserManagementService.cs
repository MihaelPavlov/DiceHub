using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Authentication.Services;

internal class UserManagementService(
    ILogger<UserManagementService> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILocalizationService localizer,
    ISynchronizeUsersChallengesQueue queue,
    IRepository<UserDeviceToken> userDeviceTokenRepository,
    IRepository<TenantUserSetting> tenantUserSettingRepository) : IUserManagementService
{
    readonly ILogger<UserManagementService> logger = logger;
    readonly UserManager<ApplicationUser> userManager = userManager;
    readonly RoleManager<IdentityRole> roleManager = roleManager;
    readonly ILocalizationService localizer = localizer;
    readonly ISynchronizeUsersChallengesQueue queue = queue;
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository = userDeviceTokenRepository;
    readonly IRepository<TenantUserSetting> tenantUserSettingRepository = tenantUserSettingRepository;

    public async Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest form)
    {
        if (!form.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var existingUserByEmail = await this.userManager.Users
             .Where(x => x.Email == form.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Email", this.localizer["UserExistEmail"]);

        var existingUserByUsername = await this.userManager.Users
             .Where(x => x.UserName == form.Username).FirstOrDefaultAsync();

        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Username", this.localizer["UserExistUsername"]);

        var user = new ApplicationUser() { UserName = form.Username, Email = form.Email };
        var createUserResult = await userManager.CreateAsync(user, form.Password);

        if (!createUserResult.Succeeded)
            throw new ValidationErrorsException("General", this.localizer["UserRegistrationFailed"]);

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == Role.User.ToString()))
        {
            this.logger.LogCritical("User role was not found during registration of user.");
            throw new BadRequestException(this.localizer["UserRegistrationFailed"]);
        }

        await this.userManager.AddToRoleAsync(user, Role.User.ToString());

        user = await this.userManager.Users
            .Where(x => x.Email == form.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotCreated"]);

        await this.queue.AddSynchronizeNewUserJob(user.Id);

        if (!string.IsNullOrEmpty(form.DeviceToken))
        {
            await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
            {
                DeviceToken = form.DeviceToken,
                LastUpdated = DateTime.UtcNow,
                UserId = user.Id
            }, CancellationToken.None);
        }

        await this.tenantUserSettingRepository.AddAsync(new TenantUserSetting
        {
            UserId = user.Id,
            Language = form.Language ?? SupportLanguages.EN.ToString(),
        }, CancellationToken.None);

        return new UserRegistrationResponse
        {
            UserId = user.Id,
        };
    }

    public async Task<UserModel?> GetUserById(string id, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName ?? this.localizer["NotProvided"],
                Email = x.Email ?? this.localizer["NotProvided"],
                PhoneNumber = x.PhoneNumber ?? this.localizer["NotProvided"],
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            user.FirstName = user.UserName.Split(", ").FirstOrDefault() ?? string.Empty;
            user.LastName = user.UserName.Split(", ").LastOrDefault() ?? string.Empty;
        }

        return user;
    }

    public async Task<UserModel?> GetUserByEmail(string email)
    {
        var user = await this.userManager.Users
            .Where(x => x.Email == email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user == null)
            return null;

        return new UserModel
        {
            Id = user!.Id,
            UserName = user.UserName ?? this.localizer["NotProvided"],
            Email = user.Email ?? this.localizer["NotProvided"],
        };
    }

    public async Task<List<GetUserByRoleModel>> GetUserListByRole(Role role, CancellationToken cancellationToken)
    {
        // Check if the role exists
        if (!await this.roleManager.RoleExistsAsync(role.ToString()))
        {
            this.logger.LogCritical("Request with Role that doesn't exists was initiated! Role: {Role}", role.ToString());
            return [];
        }

        // Get users in the specified role
        var usersInRole = await this.userManager.GetUsersInRoleAsync(role.ToString());

        return usersInRole.Where(x => !x.IsDeleted).Select(x => new GetUserByRoleModel
        {
            Id = x.Id,
            UserName = x.UserName ?? this.localizer["UsernameNotProvided"]
        }).ToList();
    }

    public async Task<List<GetUserByRoleModel>> GetUserListByRoles(Role[] roles, CancellationToken cancellationToken)
    {
        var result = new List<GetUserByRoleModel>();
        foreach (var role in roles)
        {
            // Check if the role exists
            if (!await this.roleManager.RoleExistsAsync(role.ToString()))
            {
                this.logger.LogCritical("Request with Role that doesn't exists was initiated! Role: {Role}", role.ToString());
                return [];
            }
            // Get users in the specified role
            var usersInRole = await this.userManager.GetUsersInRoleAsync(role.ToString());

            result.AddRange(usersInRole.Where(x => !x.IsDeleted).Select(x => new GetUserByRoleModel
            {
                Id = x.Id,
                UserName = x.UserName ?? this.localizer["UsernameNotProvided"]
            }).ToList());
        }

        return result;
    }

    public async Task<string[]> GetAllUserIds(CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<string> GetUserTimeZone(string userId)
    {
        var user = await this.userManager.Users
           .Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserByEmailNotFound"]);

        return user!.TimeZone;
    }

    public async Task<bool> IsUserInRole(string userId, Role role, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users
           .Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            return false;

        return await this.userManager.IsInRoleAsync(user, role.ToString());
    }

    public async Task<bool> HasUserAnyMatchingRole(string userId, params Role[] roles)
    {
        var user = await this.userManager.Users
            .Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        foreach (var role in roles)
        {
            var isInRole = await this.userManager.IsInRoleAsync(user!, role.ToString());

            if (isInRole)
                return true;
        }

        return false;
    }

    public async Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName ?? this.localizer["NotProvided"],
                Email = x.Email ?? this.localizer["NotProvided"],
                ImageUrl = string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await this.userManager.Users
           .Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        return await userManager.GenerateEmailConfirmationTokenAsync(user!);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        return await userManager.GeneratePasswordResetTokenAsync(user!);
    }

    public async Task<UserDeviceToken?> GetDeviceTokenByUserEmail(string email)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        return await this.userDeviceTokenRepository.GetByAsync(x => x.UserId == user.Id, CancellationToken.None);
    }
}
