using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Seeder;
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
    readonly IGameSeeder gameSeeder;
    readonly ISystemUserContextAccessor systemUserContextAccessor;

    public DataSeeder(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        ILogger<DataSeeder> logger,
        IEnumerable<ISeedService> seedServices,
        IGameSeeder gameSeeder,
        ISystemUserContextAccessor systemUserContextAccessor)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.seedServices = seedServices;
        this.gameSeeder = gameSeeder;
        this.systemUserContextAccessor = systemUserContextAccessor;
    }

    /// <inheritdoc/>
    public async Task SeedAsync()
    {
        await this.SeedEmailTemplatesForTenants();
        await this.SeedUniversalChallengesForTenants();

        //using (var context = await this.dbContextFactory.CreateDbContextAsync())
        //{
        //    using (var transaction = await context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GameCategories ON");

        //            var isAnyGameCategories = await context.GameCategories.AnyAsync();
        //            if (!isAnyGameCategories)
        //            {
        //                await context.AddRangeAsync(SeedData.GAME_CATEGORIES);
        //            }

        //            var isAnyUniversalChallenges = await context.UniversalChallenges.AnyAsync();
        //            if (!isAnyUniversalChallenges)
        //            {
        //                //await context.UniversalChallenges.ExecuteDeleteAsync();
        //                await context.AddRangeAsync(SeedData.UNIVERSAL_CHALLENGES);
        //            }

        //            var isAnyEmailTemplates = await context.EmailTemplates.AnyAsync();
        //            await context.EmailTemplates.ExecuteDeleteAsync();
        //            await context.AddRangeAsync(SeedData.EMAIL_TEMPLATES);

        //            await context.SaveChangesAsync();
        //            //await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT GameCategories OFF");

        //            await transaction.CommitAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            this.logger.LogError(ex, "An error occurred during the transaction while seeding database Entities. The transaction has been rolled back.");
        //        }
        //    }
        //}

        //await this.gameSeeder.SeedAsync();

        //await this.ExecuteSeeders();
    }

    async Task SeedEmailTemplatesForTenants()
    {
        try
        {
            await using var context = await this.dbContextFactory.CreateDbContextAsync();
            var tenantIds = await context.Tenants
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();

            foreach (var tenantId in tenantIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var existingTemplates = await context.EmailTemplates
                    .AsNoTracking()
                    .Where(x => x.TenantId == tenantId)
                    .Select(x => new { x.TemplateName, x.Language })
                    .ToListAsync();

                var existingKeys = existingTemplates
                    .Select(x => CreateTemplateKey(x.TemplateName, x.Language))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var missingTemplates = SeedData.EMAIL_TEMPLATES
                    .Where(x => !existingKeys.Contains(CreateTemplateKey(x.TemplateName, x.Language)))
                    .Select(x => CloneEmailTemplate(x, tenantId))
                    .ToList();

                if (missingTemplates.Count == 0)
                    continue;

                await context.EmailTemplates.AddRangeAsync(missingTemplates);
                this.systemUserContextAccessor.Set(new DataSeederSystemUserContext(tenantId));
                await context.SaveChangesAsync();

                this.logger.LogInformation(
                    "Seeded {TemplateCount} email templates for tenant {TenantId}.",
                    missingTemplates.Count,
                    tenantId);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while seeding tenant email templates.");
        }
    }

    async Task SeedUniversalChallengesForTenants()
    {
        try
        {
            await using var context = await this.dbContextFactory.CreateDbContextAsync();
            var tenantIds = await context.Tenants
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();

            foreach (var tenantId in tenantIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var existingTypes = await context.UniversalChallenges
                    .AsNoTracking()
                    .Where(x => x.TenantId == tenantId)
                    .Select(x => x.Type)
                    .ToListAsync();

                var missingChallenges = SeedData.UNIVERSAL_CHALLENGES
                    .Where(x => !existingTypes.Contains(x.Type))
                    .Select(x => CloneUniversalChallenge(x, tenantId))
                    .ToList();

                if (missingChallenges.Count == 0)
                    continue;

                await context.UniversalChallenges.AddRangeAsync(missingChallenges);
                this.systemUserContextAccessor.Set(new DataSeederSystemUserContext(tenantId));
                await context.SaveChangesAsync();

                this.logger.LogInformation(
                    "Seeded {ChallengeCount} universal challenges for tenant {TenantId}.",
                    missingChallenges.Count,
                    tenantId);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while seeding tenant universal challenges.");
        }
    }

    static EmailTemplate CloneEmailTemplate(EmailTemplate template, string tenantId) => new()
    {
        TenantId = tenantId,
        Language = template.Language,
        TemplateName = template.TemplateName,
        TemplateHtml = template.TemplateHtml,
        Subject = template.Subject,
    };

    static string CreateTemplateKey(string templateName, string language) =>
        $"{templateName.Trim()}::{language.Trim()}";

    static UniversalChallenge CloneUniversalChallenge(UniversalChallenge challenge, string tenantId) => new()
    {
        TenantId = tenantId,
        RewardPoints = challenge.RewardPoints,
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow,
        CreatedBy = "Seeder",
        UpdatedBy = "Seeder",
        Name_EN = challenge.Name_EN,
        Name_BG = challenge.Name_BG,
        Description_EN = challenge.Description_EN,
        Description_BG = challenge.Description_BG,
        Type = challenge.Type,
        Attempts = challenge.Attempts,
        MinValue = challenge.MinValue,
    };

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

    private sealed class DataSeederSystemUserContext(string tenantId) : IUserContext
    {
        public string? TenantId => tenantId;
        public string? UserId => "data-seeder";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
