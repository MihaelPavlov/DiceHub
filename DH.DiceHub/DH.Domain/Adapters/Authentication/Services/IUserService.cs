using DH.Domain.Adapters.Authentication.Models;

namespace DH.Domain.Adapters.Authentication.Services;

/// <summary>
/// Interface for user-related services, including authentication, registration,
/// role management, and token generation.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Handles user login and generates JWT token upon successful authentication.
    /// </summary>
    /// <param name="form">LoginForm containing user related information.</param>
    /// <returns> <see cref="AuthenticatedResponse"/> containing JWT token if login is successful.</returns>
    Task<TokenResponseModel> Login(LoginRequest form, bool fromRegister = false);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="form">RegisterForm containing new user infromation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Register(RegistrationRequest form);

    Task RegisterNotification(string email);

    /// <summary>
    /// Get user list only if the id is presented in the ids array.
    /// </summary>
    /// <param name="ids">Ids of user that we want.</param>
    /// <returns>A <see cref="UserModel"/> collection.</returns>
    Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken);
}
