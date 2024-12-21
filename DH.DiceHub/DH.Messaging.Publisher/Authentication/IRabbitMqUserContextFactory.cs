namespace DH.Messaging.Publisher.Authentication;

/// <summary>
/// Provides a method to create instances of the <see cref="IRabbitMqUserContext"/>.
/// </summary>
public interface IRabbitMqUserContextFactory
{

    /// <summary>
    /// Creates a new instance of the <see cref="IRabbitMqUserContext"/>.
    /// </summary>
    /// <returns>A new <see cref="IUserContext"/> instance.</returns>
    IRabbitMqUserContext CreateUserContext();

    void SetDefaultUserContext(IRabbitMqUserContext defaultUserContext);

}
