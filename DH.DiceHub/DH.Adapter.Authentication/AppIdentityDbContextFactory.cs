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
        optionsBuilder.UseSqlServer("Server=(local);Database=DH.DiceHub;Trusted_Connection=True;Encrypt=False");

        return new AppIdentityDbContext(optionsBuilder.Options, this.containerService);
    }
}
