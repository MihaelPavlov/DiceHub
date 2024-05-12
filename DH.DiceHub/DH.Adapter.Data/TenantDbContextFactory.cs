using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

public class TenantDbContextFactory : IDbContextFactory<TenantDbContext>
{
    readonly IContainerService containerService;

    public TenantDbContextFactory(IContainerService containerService)
    {
        this.containerService = containerService;
    }

    public TenantDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer("Server=(local);Database=DH.DiceHub;Trusted_Connection=True;Encrypt=False");

        return new TenantDbContext(optionsBuilder.Options, this.containerService);
    }
}
