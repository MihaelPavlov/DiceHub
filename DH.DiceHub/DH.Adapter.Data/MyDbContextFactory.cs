using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

public class MyDbContextFactory : IDbContextFactory<MyDbContext>
{
    public IContainerService containerService;
    public MyDbContextFactory(IContainerService containerService)
    {
        this.containerService = containerService;
    }
    public MyDbContext CreateDbContext()
    {
        // Create and configure the database context instance
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.UseSqlServer("Server=(local);Database=DH.DiceHub;Trusted_Connection=True;Encrypt=False");

        return new MyDbContext(optionsBuilder.Options, this.containerService);
    }
}