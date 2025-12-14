namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Factory for creating <see cref="IUserContext"/> instances.
/// </summary>
public interface IUserContextFactory
{
    /// <summary>
    /// Creates a user context from the current HTTP request.
    /// Returns an anonymous context when unauthenticated.
    /// </summary>
    Task<IUserContext> CreateAsync();

    IUserContext Create();

    IUserContext GetUserContextForB2b();
}
