using DH.Adapter.Authentication.Entities;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Adapter.Authentication;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>, IIdentityDbContext
{
    readonly IContainerService _containerService;

    public AppIdentityDbContext()
    {

    }
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IContainerService _containerService)
        : base(options)
    {
        this._containerService = _containerService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub2;User Id=app_user;Password=1qaz!QAZ;");
        }
#endif
    }

    public T AcquireRepository<T>()
    {
        return _containerService.Resolve<T>();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantEntries = ChangeTracker
            .Entries<TenantEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (tenantEntries.Count == 0)
            return base.SaveChangesAsync(cancellationToken);

        var userContext = this._containerService.Resolve<ISystemUserContextAccessor>().Current;

        // Check if the user context is anonymous
        if (userContext is AnonymousUserContext)
        {
            // Fallback to resolve IUserContext from the container (e.g., SystemUserContext or regular user context)
            userContext = this._containerService.Resolve<IUserContext>();
        }

        var hasNewTenantEntities = ChangeTracker.Entries<TenantEntity>()
            .Any(e => e.State == EntityState.Added);

        if (!userContext.IsSystem && string.IsNullOrWhiteSpace(userContext.TenantId) && hasNewTenantEntities)
        {
            throw new InvalidOperationException("TenantId is required for non-system operations");
        }

        foreach (var entry in tenantEntries)
        {
            if (entry.State == EntityState.Added && !string.IsNullOrWhiteSpace(userContext.TenantId))
                entry.Entity.TenantId = userContext.TenantId;
        }

        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            if (entry.State == EntityState.Added && string.IsNullOrWhiteSpace(entry.Entity.TenantId) && userContext.TenantId != null)
            {
                entry.Entity.TenantId = userContext.TenantId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
