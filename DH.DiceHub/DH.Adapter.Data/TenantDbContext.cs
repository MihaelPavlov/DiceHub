using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using System.Reflection;
using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

public class TenantDbContext : DbContext, ITenantDbContext 
{
    readonly IContainerService containerService;

    public TenantDbContext()
    {
    }

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,  IContainerService containerService)
        : base(options)
    {
        this.containerService = containerService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("server=(local); database=DH.DiceHub; Integrated Security=true; encrypt=false");
        }
#endif
    }

    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameReview> GameReviews { get; set; } = default!;
    public DbSet<Event> Events { get; set; } = default!;

    public T AcquireRepository<T>()
    {
        return containerService.Resolve<T>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ////TODO: Check if we ened to configure the mapping manually or the 24 line is working 
        //CommonMapping.Configure(builder);

        base.OnModelCreating(builder);
    }
}
