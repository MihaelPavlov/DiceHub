using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Database.Connector;

public class TenantDbContextFactory<TContext> : IDbContextFactory<TContext>
    where TContext : TenantDbContext
{
    private readonly string _connectionString;
    private readonly Assembly _entityAssembly;

    public TenantDbContextFactory(string connectionString, Assembly entityAssembly)
    {
        _connectionString = connectionString;
        _entityAssembly = entityAssembly;
    }

    public TContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>()
            .UseSqlServer(_connectionString)
        ?? throw new InvalidOperationException("Failed to initialize DbContextOptionsBuilder. Ensure the connection string is valid and properly configured.");

        // Use Activator to create an instance of the derived context
        var context = (TContext?)Activator.CreateInstance(
            typeof(TContext),
            optionsBuilder.Options,
            _entityAssembly
        );

        if (context == null)
            throw new Exception($"Db context from instance {typeof(TContext).Name} was not found");

        return context;
    }
}