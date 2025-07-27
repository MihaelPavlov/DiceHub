using DH.Adapter.Authentication.Entities;
using DH.Adapter.Authentication.Helper;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository;
    readonly IUserContext userContext;
    readonly ILogger<UserService> logger;
    readonly IRepository<TenantSetting> tenantSettingsRepository;

    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    public UserService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IPermissionStringBuilder permissionStringBuilder, SynchronizeUsersChallengesQueue queue, IRepository<UserDeviceToken> userDeviceTokenRepository,
        IUserContext userContext, ILogger<UserService> logger, IRepository<TenantSetting> tenantSettingsRepository)
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
    }

    /// <inheritdoc />
    public async Task<TokenResponseModel?> Login(LoginRequest form)
    {
        var user = await this.userManager.FindByEmailAsync(form.Email);

        if (user.IsInvalid())
            throw new ValidationErrorsException("Email", "Email or Password is invalid!");

        if (!await userManager.IsEmailConfirmedAsync(user!))
            throw new ValidationErrorsException("EmailNotConfirmed", "Email not confirmed. Please check your inbox.");

        var result = await this.signInManager.PasswordSignInAsync(user!, form.Password, form.RememberMe, true);

        if (!result.Succeeded)
            throw new ValidationErrorsException("Email", "Email or Password is invalid!");

        if (!string.IsNullOrEmpty(form.TimeZone) && form.TimeZone != user!.TimeZone)
        {
            user.TimeZone = form.TimeZone!;
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
        if (!form.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var existingUserByEmail = await this.userManager.FindByEmailAsync(form.Email);
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Exist", "Player with that Email, already exist");

        var existingUserByUsername = await this.userManager.FindByNameAsync(form.Username);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", "Player with that Username, already exist");

        var user = new ApplicationUser() { UserName = form.Username, Email = form.Email };
        var createUserResult = await userManager.CreateAsync(user, form.Password);
        if (!createUserResult.Succeeded)
            throw new BadRequestException("User registration failed!");

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == "User"))
            throw new BadRequestException("User registration failed!");

        await this.userManager.AddToRoleAsync(user, Role.User.ToString());

        user = await this.userManager.FindByEmailAsync(form.Email);
        if (user is null)
            throw new NotFoundException("User was not created");

        this.queue.AddSynchronizeNewUserJob(user.Id);

        if (!string.IsNullOrEmpty(form.DeviceToken))
        {
            await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
            {
                DeviceToken = form.DeviceToken,
                LastUpdated = DateTime.UtcNow,
                UserId = user.Id
            }, CancellationToken.None);
        }

        return new UserRegistrationResponse
        {
            UserId = user.Id,
        };
    }

    public async Task<TokenResponseModel?> ConfirmEmail(string email, string token, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        var result = await this.userManager.ConfirmEmailAsync(user!, token);

        if (result.Succeeded)
        {
            await this.signInManager.SignInAsync(user!, true);

            return await IssueUserTokensAsync(user!);
        }

        throw new ValidationErrorsException("InvalidToken", "Token expired!");
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        return await userManager.GenerateEmailConfirmationTokenAsync(user!);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        return await userManager.GeneratePasswordResetTokenAsync(user!);
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await this.userManager.FindByEmailAsync(request.Email);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        if (!request.NewPassword.Equals(request.ConfirmPassword))
            throw new ValidationErrorsException("Password", "Password are not identical!");

        var result = await this.userManager.ResetPasswordAsync(user!, request.Token, request.NewPassword);

        if (!result.Succeeded)
            throw new ValidationErrorsException("Password", @"Reseting Password was not succesfully, 
                try again later, or contact us via email");
    }

    public async Task<UserDeviceToken?> GetDeviceTokenByUserEmail(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user.IsInvalid())
            throw new NotFoundException("User is not found");

        return await this.userDeviceTokenRepository.GetByAsync(x => x.UserId == user!.Id, CancellationToken.None);
    }

    public async Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName ?? "Not Provided",
                Email = x.Email ?? "Not Provided",
                ImageUrl = string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasUserAnyMatchingRole(string userId, params Role[] roles)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

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
                UserName = x.UserName ?? "Not Provided",
                Email = x.Email ?? "Not Provided",
                PhoneNumber = x.PhoneNumber ?? "Not Provided",
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
        var user = await this.userManager.FindByEmailAsync(email);

        if (user.IsInvalid())
            return null;

        return new UserModel
        {
            Id = user!.Id,
            UserName = user.UserName ?? "Not Provided",
            Email = user.Email ?? "Not Provided",
        };
    }

    public async Task<List<GetUserByRoleModel>> GetUserListByRole(Role role, CancellationToken cancellationToken)
    {
        // Check if the role exists
        if (!await this.roleManager.RoleExistsAsync(role.ToString()))
            throw new ArgumentException("Role does not exist.");

        // Get users in the specified role
        var usersInRole = await this.userManager.GetUsersInRoleAsync(role.ToString());

        return usersInRole.Where(x => !x.IsDeleted).Select(x => new GetUserByRoleModel
        {
            Id = x.Id,
            UserName = x.UserName ?? "username_placeholder",
        }).ToList();
    }

    public async Task<List<GetUserByRoleModel>> GetUserListByRoles(Role[] roles, CancellationToken cancellationToken)
    {
        var result = new List<GetUserByRoleModel>();
        foreach (var role in roles)
        {
            // Check if the role exists
            if (!await this.roleManager.RoleExistsAsync(role.ToString()))
                throw new ArgumentException("Role does not exist.");

            // Get users in the specified role
            var usersInRole = await this.userManager.GetUsersInRoleAsync(role.ToString());

            result.AddRange(usersInRole.Where(x => !x.IsDeleted).Select(x => new GetUserByRoleModel
            {
                Id = x.Id,
                UserName = x.UserName ?? "username_placeholder",
            }).ToList());
        }

        return result;
    }

    public async Task<EmployeeResult> CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!await this.HasUserAnyMatchingRole(this.userContext.UserId, Role.SuperAdmin, Role.Owner))
            throw new BadRequestException("Only owner can create new employees");

        if (!request.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var existingUserByEmail = await this.userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Exist", "Player with that Email, already exist");

        var username = $"{request.FirstName}, {request.LastName}";
        var existingUserByUsername = await this.userManager.FindByNameAsync(username);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", "A player with the same first and last name already exists.");

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
            throw new BadRequestException("User registration failed!");

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == Role.Staff.ToString()))
            throw new BadRequestException("User registration failed!");

        await this.userManager.AddToRoleAsync(user, Role.Staff.ToString());

        var afterRegister = await this.userManager.FindByEmailAsync(request.Email);
        if (afterRegister is null)
            throw new NotFoundException("User was not created");

        this.queue.AddSynchronizeNewUserJob(afterRegister.Id);

        return new EmployeeResult
        {
            UserId = afterRegister.Id,
            Email = afterRegister.Email!,
        };
    }

    public async Task<OwnerResult> CreateOwner(CreateOwnerRequest request, CancellationToken cancellationToken)
    {
        if (!await this.HasUserAnyMatchingRole(this.userContext.UserId, Role.SuperAdmin))
            throw new BadRequestException("Only SuperAdmin can add owner");

        if (!request.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var owner = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());
        if (owner.Count > 1)
            throw new ValidationErrorsException("Owner", "Owner already exist, already exist");

        var existingUserByEmail = await this.userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
            throw new ValidationErrorsException("Exist", "Player with that Email, already exist");

        var username = request.Email;
        var existingUserByUsername = await this.userManager.FindByNameAsync(username);
        if (existingUserByUsername != null)
            throw new ValidationErrorsException("Exist", "A player with the same first and last name already exists.");

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
            throw new BadRequestException("User registration failed!");

        if (!await this.roleManager.Roles.AnyAsync(x => x.Name == Role.Owner.ToString()))
            throw new BadRequestException("User registration failed!");

        await this.userManager.AddToRoleAsync(user, Role.Owner.ToString());

        var afterRegister = await this.userManager.FindByEmailAsync(request.Email);
        if (afterRegister is null)
            throw new NotFoundException("User was not created");

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
        var user = await this.userManager.FindByEmailAsync(request.Email);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        if (!request.PhoneNumber.Equals(request.PhoneNumber))
            throw new ValidationErrorsException("PhoneNumber", "Provided phone number does not match the one associated with this account");

        if (!request.NewPassword.Equals(request.ConfirmPassword))
            throw new ValidationErrorsException("Password", "Password are not identical!");

        var result = await this.userManager.ResetPasswordAsync(user!, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var isTokenInvalid = result.Errors.Select(x => x.Description).Any(x => x.Contains("Invalid token"));

            if (isTokenInvalid)
            {
                throw new ValidationErrorsException(
                    "InvalidToken",
                    "The password reset link is either invalid or has expired. Please request a new one or contact support if the issue persists.");
            }

            this.logger.LogError(
                "CreateEmployeePassword failed for user with email: {Email}. Errors: {Errors}",
                request.Email,
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
            throw new ValidationErrorsException("Password", @"Oops! Something went wrong while setting the password. Please try again, or reach out to our support team via email if the problem continues.");
        }
    }

    public async Task CreateOwnerPassword(CreateOwnerPasswordRequest request)
    {
        var user = await this.userManager.FindByEmailAsync(request.Email);
        if (user.IsInvalid())
            throw new NotFoundException("User was not found");

        var dbSettings = await this.tenantSettingsRepository.GetByAsync(x => x.Id == 1, CancellationToken.None);
        if (dbSettings != null)
        {
            if (!request.ClubPhoneNumber.Equals(dbSettings.PhoneNumber))
                throw new ValidationErrorsException(
                    "ClubPhoneNumber",
                    "Provided club phone number does not match the one associated with this account");
        }

        if (!request.NewPassword.Equals(request.ConfirmPassword))
            throw new ValidationErrorsException("Password", "Password are not identical!");

        var result = await this.userManager.ResetPasswordAsync(user!, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var isTokenInvalid = result.Errors.Select(x => x.Description).Any(x => x.Contains("Invalid token"));

            if (isTokenInvalid)
            {
                throw new ValidationErrorsException(
                    "InvalidToken",
                    "The password reset link is either invalid or has expired. Please request a new one or contact support if the issue persists.");
            }

            this.logger.LogError(
               "CreateOwnerPassword failed for user with email: {Email}. Errors: {Errors}",
               request.Email,
               string.Join("; ", result.Errors.Select(x => x.Description))
           );

            throw new ValidationErrorsException(
                "Password",
                @"Oops! Something went wrong while setting the password. Please try again, or reach out to our support team via email if the problem continues.");
        }
    }

    private async Task<TokenResponseModel?> IssueUserTokensAsync(ApplicationUser user)
    {
        var roles = await this.userManager.GetRolesAsync(user);
        this.logger.LogWarning("Roles ->, {roles}", string.Join(", ", roles));
        var role = roles.FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid,user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("TimeZone", user.TimeZone!),
        };
        if (role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, RoleHelper.GetRoleKeyByName(role).ToString()));
            claims.Add(new Claim("permissions", _permissionStringBuilder.GetFromCacheOrBuildPermissionsString(RoleHelper.GetRoleKeyByName(role))));
        }
        var tokenString = this.jwtService.GenerateAccessToken(claims);
        var refreshToken = this.jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

        await this.userManager.UpdateAsync(user);

        _httpContextAccessor.HttpContext!.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return new TokenResponseModel { AccessToken = tokenString, RefreshToken = refreshToken };
    }

    public async Task DeleteEmployee(string employeeId)
    {
        var user = await this.userManager.FindByIdAsync(employeeId)
            ?? throw new BadRequestException("Employee deletion failed!");

        user.IsDeleted = true;
        await this.userManager.UpdateAsync(user);
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
            throw new BadRequestException("Only owner can update employee");

        if (!request.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var existingUser = await this.userManager.FindByIdAsync(request.Id);
        if (existingUser == null)
            throw new NotFoundException("User was not found");

        var isEmailChanged = false;
        var oldEmail = string.Empty;
        if (existingUser.Email != request.Email)
        {
            var existingUserByEmail = await this.userManager.FindByEmailAsync(request.Email);

            if (existingUserByEmail != null)
                throw new ValidationErrorsException("Exist", "Player with that Email, already exist");

            isEmailChanged = true;
            oldEmail = existingUser.Email;
            existingUser.Email = request.Email;
        }

        var newUsername = $"{request.FirstName}, {request.LastName}";

        if (newUsername != existingUser.UserName)
        {
            var existingUserByUsername = await this.userManager.FindByNameAsync(newUsername);
            if (existingUserByUsername != null)
                throw new ValidationErrorsException("Exist", "A player with the same first and last name already exists.");

            existingUser.UserName = newUsername;
        }

        existingUser.PhoneNumber = request.PhoneNumber;

        var updatedUserResult = await userManager.UpdateAsync(existingUser);
        if (!updatedUserResult.Succeeded)
            throw new BadRequestException("User registration failed!");

        return new EmployeeResult
        {
            UserId = existingUser.Id,
            Email = existingUser.Email!,
            IsEmailChanged = isEmailChanged,
        };
    }

    public async Task<string[]> GetAllUserIds(CancellationToken cancellationToken)
    {
        return await this.userManager.Users.Select(x => x.Id).ToArrayAsync(cancellationToken);
    }

    public async Task<OwnerResult?> GetOwner(CancellationToken cancellationToken)
    {
        // Check if the role exists
        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
            throw new InfrastructureException("Role Owner does not exist.");

        // Get users in the specified role
        var usersInRole = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());

        if (usersInRole == null)
            return null;

        if (usersInRole.Count() > 1)
            throw new InfrastructureException("More then one Owner are founded.");

        return new OwnerResult
        {
            Email = usersInRole.First().Email!
        };
    }

    public async Task DeleteOwner(CancellationToken cancellationToken)
    {
        // Check if the role exists
        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
            throw new InfrastructureException("Role Owner does not exist.");

        // Get users in the specified role
        var usersInRole = await this.userManager.GetUsersInRoleAsync(Role.Owner.ToString());

        if (usersInRole == null)
            throw new InfrastructureException("Owner for deletion not found.");

        if (usersInRole.Count() > 1)
            throw new InfrastructureException("More then one Owner are founded.");

        await this.userManager.DeleteAsync(usersInRole.First());
    }
}
