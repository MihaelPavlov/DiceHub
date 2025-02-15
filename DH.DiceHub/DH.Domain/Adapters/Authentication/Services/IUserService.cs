﻿using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Entities;

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
    Task<TokenResponseModel?> Login(LoginRequest form);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="form">RegisterForm containing new user infromation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RegisterUser(UserRegistrationRequest form);

    Task<UserDeviceToken> GetDeviceTokenByUserEmail(string email);

    /// <summary>
    /// Get user list only if the id is presented in the ids array.
    /// </summary>
    /// <param name="ids">Ids of user that we want.</param>
    /// <returns>A <see cref="UserModel"/> collection.</returns>
    Task<List<UserModel>> GetUserListByIds(string[] ids, CancellationToken cancellationToken);

    /// <summary>
    /// Get user list by role
    /// </summary>
    /// <param name="role">Specific role.</param>
    /// <returns>A <see cref="GetUserByRoleModel"/> collection.</returns>
    Task<List<GetUserByRoleModel>> GetUserListByRole(Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Get user list by roles
    /// </summary>
    /// <param name="role">Specific role.</param>
    /// <returns>A <see cref="GetUserByRoleModel"/> collection.</returns>
    Task<List<GetUserByRoleModel>> GetUserListByRoles(Role[] roles, CancellationToken cancellationToken);

    /// <summary>
    /// Create employee
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken);
}
