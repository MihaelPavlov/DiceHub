using DH.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace DH.Adapter.Authentication;

public class AppIdentityDbContextFactory : IDbContextFactory<AppIdentityDbContext>
{
    readonly IContainerService containerService;
    readonly IConfiguration configuration;

    public AppIdentityDbContextFactory(IContainerService containerService, IConfiguration configuration)
    {
        this.containerService = containerService;
        this.configuration = configuration;
    }

    public AppIdentityDbContext CreateDbContext()
    {
        var connectionString = this.configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidConfigurationException("DefaultConnection: Was not found. Place AppIdentityDbContextFactory");

        var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppIdentityDbContext(optionsBuilder.Options, this.containerService, this.configuration);
    }
}
