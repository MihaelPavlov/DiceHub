﻿using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Authentication.Helper;

/// <summary>
/// Implementation of <see cref="IUserContext"/>.
/// Provides the current user's context, including user ID and authentication status.
/// </summary>
public class UserContext : IUserContext
{
    readonly string? _userId;
    readonly int? _roleKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContext"/> class with the specified user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the current user.</param>
    public UserContext(string? userId, int? roleKey)
    {
        _userId = userId;
        _roleKey = roleKey;
    }

    /// <inheritdoc/>
    public string UserId
    {
        get
        {
            if (_userId != null)
                return _userId;

            throw new Exception("Can not find current user.");
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => _userId != null;

    public int RoleKey
    {
        get
        {
            if (_roleKey != null)
                return _roleKey.Value;

            throw new Exception("Can not find current role.");
        }
    }
}
