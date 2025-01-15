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

    TenantDbContext IDbContextFactory<TenantDbContext>.CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub;User Id=postgres;Password=1qaz!QAZ;");

        return new TenantDbContext(optionsBuilder.Options, this.containerService);
    }
}
