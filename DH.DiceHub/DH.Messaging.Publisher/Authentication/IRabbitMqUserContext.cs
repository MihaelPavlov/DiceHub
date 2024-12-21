namespace DH.Messaging.Publisher.Authentication;

/// <summary>
/// Provides properties to access information about the current user context.
/// </summary>
public interface IRabbitMqUserContext
{

    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    string UserId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets a value indicating user role key.
    /// </summary>
    int RoleKey { get; set; }

    string Token { get; set; }
}


