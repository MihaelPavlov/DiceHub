using DH.Domain.Adapters.Data;
using DH.Domain.Services.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Data;

/// <inheritdoc/>
public class DataSeeder : IDataSeeder
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory;
    readonly ILogger<DataSeeder> logger;
    readonly IEnumerable<ISeedService> seedServices;

    public DataSeeder(IDbContextFactory<TenantDbContext> dbContextFactory, ILogger<DataSeeder> logger, IEnumerable<ISeedService> seedServices)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.seedServices = seedServices;
    }

    /// <inheritdoc/>
    public async Task SeedAsync()
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync())
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GameCategories ON");

                    var isAnyGameCategories = await context.GameCategories.AnyAsync();
                    if (!isAnyGameCategories)
                    {
                        await context.AddRangeAsync(SeedData.GAME_CATEGORIES);
                    }

                    var isAnyEmailTemplates = await context.EmailTemplates.AnyAsync();
                    await context.EmailTemplates.ExecuteDeleteAsync();
                    await context.AddRangeAsync(SeedData.EMAIL_TEMPLATES);

                    await context.SaveChangesAsync();
                    //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GameCategories OFF");

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    this.logger.LogError(ex, "An error occurred during the transaction while seeding database Entities. The transaction has been rolled back.");
                }
            }
        }

        await this.ExecuteSeeders();
    }

    /*
      The execution of seedServices should be placed outside of the DbContext usage scope.
      Reason:
        EF Core does not allow multiple parallel operations on the same DbContext instance.
        Since seeding services may perform database operations, executing them inside the DbContext block 
        could result in concurrency issues or conflicts.
        To avoid this, we dispose of the DbContext before invoking seed services, ensuring each 
        database operation runs in isolation.
     */
    private async Task ExecuteSeeders()
    {
        try
        {
            foreach (var seedService in this.seedServices)
            {
                await seedService.Seed();
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while executing seed services after the database transaction. There could be inconsistent data!");
        }
    }
}
