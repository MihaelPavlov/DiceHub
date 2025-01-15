using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Authentication;

public class AppIdentityDbContextFactory : IDbContextFactory<AppIdentityDbContext>
{
    readonly IContainerService containerService;

    public AppIdentityDbContextFactory(IContainerService containerService)
    {
        this.containerService = containerService;
    }

    public AppIdentityDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub;User Id=postgres;Password=1qaz!QAZ;");

        return new AppIdentityDbContext(optionsBuilder.Options, this.containerService);
    }
}
