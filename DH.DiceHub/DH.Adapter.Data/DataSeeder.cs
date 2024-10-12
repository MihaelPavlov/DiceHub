using DH.Domain.Adapters.Data;
using DH.Domain.Services.Seed;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

internal class DataSeeder : IDataSeeder
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;
    readonly IEnumerable<ISeedService> seedServices;

    public DataSeeder(IDbContextFactory<TenantDbContext> dbContextFactory, IEnumerable<ISeedService> seedServices)
    {
        this.dbContextFactory = dbContextFactory;
        this.seedServices = seedServices;
    }

    public async Task SeedAsync()
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync())
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!context.GameCategories.Any())
                    {
                        context.AddRange(SeedData.GAME_CATEGORIES);
                    }

                    foreach (var seedService in this.seedServices)
                    {
                        seedService.Seed();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
