using DH.Database.Connector.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Database.Connector;

public abstract class TenantDbContext : DbContext
{
    private readonly Assembly _entityAssembly;

    protected TenantDbContext(DbContextOptions options, Assembly entityAssembly)
        : base(options)
    {
        _entityAssembly = entityAssembly;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Register all entities that implement IDatabaseEntity dynamically
        var entityTypes = _entityAssembly.GetTypes()
            .Where(t => typeof(IDatabaseEntity).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType);
        }

        base.OnModelCreating(modelBuilder);
    }
}