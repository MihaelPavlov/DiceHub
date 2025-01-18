using DH.Adapter.Authentication.Entities;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.Reflection;

namespace DH.Adapter.Authentication;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>, IIdentityDbContext
{
    readonly IContainerService _containerService;
    readonly IConfiguration configuration;

    public AppIdentityDbContext()
    {

    }
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IContainerService _containerService, IConfiguration configuration)
        : base(options)
    {
        this._containerService = _containerService;
        this.configuration = configuration;
    }

    public DbSet<Test> Tests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        var connectionString = this.configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidConfigurationException("DefaultConnection: Was not found. Place TenantDbContextFactory");

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
#endif
    }

    public T AcquireRepository<T>()
    {
        return _containerService.Resolve<T>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
