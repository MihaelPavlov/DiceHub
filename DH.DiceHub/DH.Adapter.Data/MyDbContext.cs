using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using System.Reflection;
using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

public class MyDbContext : DbContext, IDBContext 
{
    readonly IContainerService _containerService;

    public MyDbContext()
    {
    }
    public MyDbContext(
        DbContextOptions<MyDbContext> options,  IContainerService _containerService)
        : base(options)
    {
        this._containerService = _containerService;
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

    public T AcquireRepository<T>()
    {
        return _containerService.Resolve<T>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ////TODO: Check if we ened to configure the mapping manually or the 24 line is working 
        //CommonMapping.Configure(builder);

        base.OnModelCreating(builder);
    }
}
