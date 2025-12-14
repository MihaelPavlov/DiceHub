namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Provides properties to access information about the current user context.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Gets the time zone of the current user.
    /// </summary>
    string TimeZone { get; }

    /// <summary>
    /// Preferred Language from the user
    /// </summary>
    string Language { get; }

    /// <summary>
    /// Tenant Id Of the current user
    /// </summary>
    string TenantId { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets a value indicating user role key.
    /// </summary>
	int RoleKey { get; }

    string Token { get; }
}
