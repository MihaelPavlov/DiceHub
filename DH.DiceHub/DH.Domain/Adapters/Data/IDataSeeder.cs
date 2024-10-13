namespace DH.Domain.Adapters.Data;

/// <summary>
/// Service responsible for seeding initial data into the database using a factory to create instances of <see cref="ITenantDbContext"/>.
/// The class supports multiple seed services and handles transactional operations for seeding game categories and other data.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seeds the database with initial data, including setting identity inserts for entities and invoking additional seed services.
    /// A transaction is used to ensure consistency when seeding data, and changes are committed if the operation succeeds.
    /// If an error occurs, the transaction is rolled back.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Catches exceptions that occur during database seeding operations.</exception>
    Task SeedAsync();
}
