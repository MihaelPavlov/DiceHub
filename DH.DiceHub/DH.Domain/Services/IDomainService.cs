namespace DH.Domain.Services;

/// <summary>
/// Represents a domain service for handling business logic related to entities of the specified type.
/// It give us the opportunity to handle complex transaction and manage transactions if they are necessary from the Implement Data.Adapter
/// </summary>
/// <typeparam name="T">The type of the entity the domain service operates on.</typeparam>
public interface IDomainService<T>
{
}
