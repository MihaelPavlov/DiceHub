namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Provides information about the current execution user.
/// Safe for authenticated, anonymous, and system contexts.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the current user.
    /// Null when the user is anonymous or system-level.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the unique identifier of the current user.
    /// Null when the user is anonymous or system-level.
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Gets the role key of the current user.
    /// Null when unauthenticated.
    /// </summary>
    int? RoleKey { get; }

    /// <summary>
    /// Gets the user's preferred time zone.
    /// Null when not available.
    /// </summary>
    string? TimeZone { get; }

    /// <summary>
    /// Gets the user's preferred language.
    /// Null when not available.
    /// </summary>
    string? Language { get; }

    /// <summary>
    /// Indicates whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Is system
    /// </summary>
    bool IsSystem { get; }
}
