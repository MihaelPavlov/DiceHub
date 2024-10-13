namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// Provides a method to create instances of the <see cref="IUserContext"/>.
/// </summary>
public interface IUserContextFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="IUserContext"/>.
    /// </summary>
    /// <returns>A new <see cref="IUserContext"/> instance.</returns>
    IUserContext CreateUserContext();

    void SetDefaultUserContext(IUserContext defaultUserContext);
}
