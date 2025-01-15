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

    public DbSet<Test> Tests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub;User Id=postgres;Password=1qaz!QAZ;");
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
