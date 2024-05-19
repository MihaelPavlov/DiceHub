namespace DH.Adapter.Authentication.Services;

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
    Task<AuthenticatedResponse> Login(LoginForm form);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="form">RegisterForm containing new user infromation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Register(RegisterForm form);
}
