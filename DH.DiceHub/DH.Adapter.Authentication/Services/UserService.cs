using Azure.Core;
using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
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
    readonly ISynchronizeUsersChallengesQueue queue;
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository;
    readonly IUserContext userContext;
    readonly ILogger<UserService> logger;
    readonly IRepository<TenantSetting> tenantSettingsRepository;
    readonly IRepository<TenantUserSetting> tenantUserSettingRepository;
    readonly ILocalizationService localizer;

    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    public UserService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IPermissionStringBuilder permissionStringBuilder, ISynchronizeUsersChallengesQueue queue, IRepository<UserDeviceToken> userDeviceTokenRepository,
        IUserContext userContext, ILogger<UserService> logger, IRepository<TenantSetting> tenantSettingsRepository, IRepository<TenantUserSetting> tenantUserSettingRepository, ILocalizationService localizer)
    {
        _httpContextAccessor = httpContextAccessor;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this._permissionStringBuilder = permissionStringBuilder;
        this.queue = queue;
        this.userDeviceTokenRepository = userDeviceTokenRepository;
        this.userContext = userContext;
        this.logger = logger;
        this.tenantSettingsRepository = tenantSettingsRepository;
        this.tenantUserSettingRepository = tenantUserSettingRepository;
        this.localizer = localizer;
    }

    /// <inheritdoc />
    public async Task<TokenResponseModel?> Login(LoginRequest form)
    {
        var user = await this.userManager.Users
            .Where(x => x.Email == form.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user == null)
            throw new ValidationErrorsException("Email", this.localizer["InvalidEmailOrPass"]);

        if (!await userManager.IsEmailConfirmedAsync(user!))
            throw new ValidationErrorsException("EmailNotConfirmed", this.localizer["EmailNotConfirmed"]);

        if (!await userManager.CheckPasswordAsync(user!, form.Password))
            throw new ValidationErrorsException("Email", localizer["InvalidEmailOrPass"]);

        if (!string.IsNullOrEmpty(form.TimeZone) && form.TimeZone != user!.TimeZone)
        {
            user.TimeZone = form.TimeZone!;
            await userManager.UpdateAsync(user);
        }

        var userDiviceToken = await this.userDeviceTokenRepository.GetByAsyncWithTracking(x => x.UserId == user!.Id, CancellationToken.None);
        if (userDiviceToken is null && form.DeviceToken is not null)
        {
            await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
            {
                DeviceToken = form.DeviceToken,
                LastUpdated = DateTime.UtcNow,
                UserId = user!.Id
            }, CancellationToken.None);
        }
        else if (userDiviceToken is not null && !string.IsNullOrEmpty(form.DeviceToken))
        {
            userDiviceToken.DeviceToken = form.DeviceToken;
            await this.userDeviceTokenRepository.SaveChangesAsync(CancellationToken.None);
        }

        return await IssueUserTokensAsync(user!);
    }

    /// <inheritdoc />
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

    public async Task<TokenResponseModel?> ConfirmEmail(string email, string token, CancellationToken cancellationToken)
    {
        var user = await this.userManager.Users
            .Where(x => x.Email == email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserByEmailNotFound"]);

        var result = await this.userManager.ConfirmEmailAsync(user!, token);

        if (result.Succeeded)
        {
            await this.signInManager.SignInAsync(user!, true);

            return await IssueUserTokensAsync(user!);
        }

        throw new ValidationErrorsException("InvalidToken", this.localizer["ConfirmEmailInvalidToken"]);
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

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == request.Email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        if (!request.NewPassword.Equals(request.ConfirmPassword))
            throw new ValidationErrorsException("Password", this.localizer["PasswordMismatch"]);

        var result = await this.userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
            throw new ValidationErrorsException("Password", this.localizer["PasswordResetFailed"]);
    }

    public async Task<UserDeviceToken?> GetDeviceTokenByUserEmail(string email)
    {
        var user = await this.userManager.Users
           .Where(x => x.Email == email && !x.IsDeleted).FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(this.localizer["UserNotFound"]);

        return await this.userDeviceTokenRepository.GetByAsync(x => x.UserId == user.Id, CancellationToken.None);
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

    public async Task<UserModel?> GetEmployeeById(string id, CancellationToken cancellationToken)
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

    public async Task<EmployeeResult> CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await this.HasUserAnyMatchingRole(this.userContext.UserId, Role.SuperAdmin, Role.Owner))
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
        var generatedRandomPassword = GenerateRandomPassword();
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

    public async Task<OwnerResult> CreateOwner(CreateOwnerRequest request, CancellationToken cancellationToken)
    {
        if (!await this.HasUserAnyMatchingRole(this.userContext.UserId, Role.SuperAdmin))
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
        var generatedRandomPassword = GenerateRandomPassword();
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

    private async Task<TokenResponseModel?> IssueUserTokensAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("TimeZone", user.TimeZone!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (roleName != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            claims.Add(new Claim("role_key",
                RoleHelper.GetRoleKeyByName(roleName).ToString()));

            claims.Add(new Claim(
                "permissions",
                this._permissionStringBuilder
                    .GetFromCacheOrBuildPermissionsString(
                        RoleHelper.GetRoleKeyByName(roleName))));

        }
        var accessToken = this.jwtService.GenerateAccessToken(claims);
        var refreshToken = this.jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

        await this.userManager.UpdateAsync(user);

        return new TokenResponseModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id
        };
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

    private static string GenerateRandomPassword()
    {
        var passwordLength = 12;
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:<>?/";

        return new string(Enumerable.Range(0, passwordLength)
            .Select(_ => chars[new Random().Next(chars.Length)])
            .ToArray());
    }

    public async Task<EmployeeResult> UpdateEmployee(UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await this.HasUserAnyMatchingRole(this.userContext.UserId, Role.SuperAdmin, Role.Owner))
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

    public async Task<string[]> GetAllUserIds(CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
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

    public async Task Logout(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return;

        // Invalidate refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        // Optional: kill all sessions
        user.SecurityStamp = Guid.NewGuid().ToString();

        await userManager.UpdateAsync(user);
    }
}
