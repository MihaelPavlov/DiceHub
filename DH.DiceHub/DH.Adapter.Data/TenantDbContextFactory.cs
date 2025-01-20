using DH.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace DH.Adapter.Data;

public class TenantDbContextFactory : IDbContextFactory<TenantDbContext>
{
    readonly IContainerService containerService;
    readonly IConfiguration configuration;

    public TenantDbContextFactory(IContainerService containerService, IConfiguration configuration)
    {
        this.containerService = containerService;
        this.configuration = configuration;
    }

    TenantDbContext IDbContextFactory<TenantDbContext>.CreateDbContext()
    {
        var connectionString = this.configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidConfigurationException("DefaultConnection: Was not found. Place TenantDbContextFactory");

        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new TenantDbContext(optionsBuilder.Options, this.containerService);
    }
}
