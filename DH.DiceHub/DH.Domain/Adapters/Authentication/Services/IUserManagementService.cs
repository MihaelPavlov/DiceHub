using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Entities;

namespace DH.Domain.Adapters.Authentication.Services;

public interface IUserManagementService
{
    Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest form);
    Task<string> GetUserTimeZone(string userId);
    Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken);
    Task<string[]> GetAllUserIds(CancellationToken cancellationToken);
    Task<UserModel?> GetUserById(string id, CancellationToken cancellationToken);
    Task<UserModel?> GetUserByEmail(string email);
    Task<List<GetUserByRoleModel>> GetUserListByRole(Role role, CancellationToken cancellationToken);
    Task<List<GetUserByRoleModel>> GetUserListByRoles(Role[] roles, CancellationToken cancellationToken);
    Task<bool> IsUserInRole(string userId, Role role, CancellationToken cancellationToken);
    Task<bool> HasUserAnyMatchingRole(string userId, params Role[] roles);
    Task<UserDeviceToken?> GetDeviceTokenByUserEmail(string email);
    Task<string> GeneratePasswordResetTokenAsync(string email);
    Task<string> GenerateEmailConfirmationTokenAsync(string userId);
}
