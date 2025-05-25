
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
using System;
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
    readonly IPushNotificationsService pushNotificationsService;
    readonly IRepository<UserDeviceToken> userDeviceTokenRepository;
    readonly IUserContext userContext;

    /// <summary>
    /// Constructor for UserService to initialize dependencies.
    /// </summary>
    public UserService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
        SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IPermissionStringBuilder permissionStringBuilder, SynchronizeUsersChallengesQueue queue,
        IPushNotificationsService pushNotificationsService, IRepository<UserDeviceToken> userDeviceTokenRepository,
        IUserContext userContext)
    {
        _httpContextAccessor = httpContextAccessor;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this._permissionStringBuilder = permissionStringBuilder;
        this.queue = queue;
        this.pushNotificationsService = pushNotificationsService;
        this.userDeviceTokenRepository = userDeviceTokenRepository;
        this.userContext = userContext;
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

        var userDiviceToken = await this.userDeviceTokenRepository.GetByAsyncWithTracking(x => x.UserId == user!.Id, CancellationToken.None);
        if (userDiviceToken is null)
        {
            await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
            {
                DeviceToken = form.DeviceToken,
                LastUpdated = DateTime.UtcNow,
                UserId = user!.Id
            }, CancellationToken.None);
        }
        else if (!string.IsNullOrEmpty(form.DeviceToken))
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
        await this.userDeviceTokenRepository.AddAsync(new UserDeviceToken
        {
            DeviceToken = form.DeviceToken,
            LastUpdated = DateTime.UtcNow,
            UserId = user.Id
        }, CancellationToken.None);

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

    public async Task<UserDeviceToken> GetDeviceTokenByUserEmail(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user.IsInvalid())
            throw new ArgumentNullException("User is not found");

        var userDeviceToken = await this.userDeviceTokenRepository.GetByAsync(x => x.UserId == user!.Id, CancellationToken.None);

        if (userDeviceToken is null)
            throw new NotFoundException("User Device Token was not found");
        return userDeviceToken;
    }

    public async Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken)
    {
        return await this.userManager.Users
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName ?? "NOT_PROVIDED",
                Email = x.Email ?? "NOT_PROVIDED",
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
                UserName = x.UserName ?? "NOT_PROVIDED",
                Email = x.Email ?? "NOT_PROVIDED",
                PhoneNumber = x.PhoneNumber ?? "NOT_PROVIDED",
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
            UserName = user.UserName ?? "NOT_PROVIDED",
            Email = user.Email ?? "NOT_PROVIDED",
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
            throw new ValidationErrorsException("Password", @"Create Employee Password was not succesfully, 
                try again later, or contact us via email");
    }

    private async Task<TokenResponseModel?> IssueUserTokensAsync(ApplicationUser user)
    {
        var roles = await this.userManager.GetRolesAsync(user);
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid,user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, RoleHelper.GetRoleKeyByName(roles.First()).ToString()),
                new Claim("permissions",_permissionStringBuilder.GetFromCacheOrBuildPermissionsString( RoleHelper.GetRoleKeyByName(roles.First())))
            };

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
            Email = existingUser.Email!,
            IsEmailChanged = isEmailChanged,
        };
    }
}
