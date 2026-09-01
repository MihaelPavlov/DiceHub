using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using System.Reflection;
using DH.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DH.Domain.Adapters.Authentication;

namespace DH.Adapter.Data;

public class TenantDbContext : DbContext, ITenantDbContext
{
    readonly IContainerService containerService;

    private IHttpContextAccessor HttpContextAccessor =>
        containerService.Resolve<IHttpContextAccessor>();

    private string? CurrentTenantId
    {
        get
        {
            var requestTenant = HttpContextAccessor.HttpContext?.Items["TenantId"]?.ToString();
            if (!string.IsNullOrWhiteSpace(requestTenant))
                return requestTenant;

            return containerService?.Resolve<ISystemUserContextAccessor>().Peek.TenantId;
        }
    }

    private bool IsSystemContext =>
        containerService?.Resolve<ISystemUserContextAccessor>()?.Peek.IsSystem == true;

    public TenantDbContext()
    {
    }

    public TenantDbContext(DbContextOptions<TenantDbContext> options)
       : base(options)
    {
    }
    public TenantDbContext(
       DbContextOptions<TenantDbContext> options, IContainerService containerService)
       : base(options)
    {
        this.containerService = containerService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub2;User Id=app_user;Password=1qaz!QAZ;");
        }
#endif
    }

    #region games

    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameReview> GameReviews { get; set; } = default!;
    public DbSet<GameLike> GameLikes { get; set; } = default!;
    public DbSet<GameCategory> GameCategories { get; set; } = default!;
    public DbSet<GameReservation> GameReservations { get; set; } = default!;
    public DbSet<GameInventory> GameInventories { get; set; } = default!;
    public DbSet<SeedGameCatalog> SeedGameCatalog { get; set; } = default!;

    #endregion games

    #region events

    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<EventParticipant> EventParticipants { get; set; } = default!;

    #endregion events

    #region rooms

    public DbSet<Room> Rooms { get; set; } = default!;
    public DbSet<RoomParticipant> RoomParticipants { get; set; } = default!;
    public DbSet<RoomMessage> RoomMessages { get; set; } = default!;
    public DbSet<RoomInfoMessage> RoomInfoMessages { get; set; } = default!;

    #endregion rooms

    #region challenges

    public DbSet<Challenge> Challenges { get; set; } = default!;
    public DbSet<UniversalChallenge> UniversalChallenges { get; set; } = default!;
    public DbSet<ChallengeStatistic> ChallengeStatistics { get; set; } = default!;
    public DbSet<UserChallenge> UserChallenges { get; set; } = default!;
    public DbSet<ChallengeReward> ChallengeRewards { get; set; } = default!;
    public DbSet<UserChallengeReward> UserChallengeRewards { get; set; } = default!;
    public DbSet<UserChallengePeriodReward> UserChallengePeriodRewards { get; set; } = default!;
    public DbSet<UserChallengePeriodPerformance> UserChallengePeriodPerformances { get; set; } = default!;
    public DbSet<CustomPeriodReward> CustomPeriodRewards { get; set; } = default!;
    public DbSet<CustomPeriodChallenge> CustomPeriodChallenges { get; set; } = default!;
    public DbSet<CustomPeriodUniversalChallenge> CustomPeriodUniversalChallenges { get; set; } = default!;
    public DbSet<CustomPeriodUserChallenge> CustomPeriodUserChallenges { get; set; } = default!;
    public DbSet<CustomPeriodUserUniversalChallenge> CustomPeriodUserUniversalChallenges { get; set; } = default!;
    public DbSet<CustomPeriodUserReward> CustomPeriodUserRewards { get; set; } = default!;

    #endregion challenges

    #region space

    public DbSet<SpaceTable> SpaceTables { get; set; } = default!;
    public DbSet<SpaceTableReservation> SpaceTableReservations { get; set; } = default!;
    public DbSet<SpaceTableParticipant> SpaceTableParticipants { get; set; } = default!;

    #endregion space

    #region Statistics

    public DbSet<ClubVisitorLog> ClubVisitorLogs { get; set; } = default!;
    public DbSet<EventAttendanceLog> EventAttendanceLogs { get; set; } = default!;
    public DbSet<ReservationOutcomeLog> ReservationOutcomeLogs { get; set; } = default!;
    public DbSet<RewardHistoryLog> RewardHistoryLogs { get; set; } = default!;
    public DbSet<ChallengeHistoryLog> ChallengeHistoryLogs { get; set; } = default!;
    public DbSet<GameEngagementLog> GameEngagementLogs { get; set; } = default!;

    #endregion Statistics

    #region email

    public DbSet<EmailTemplate> EmailTemplates { get; set; } = default!;
    public DbSet<EmailHistory> EmailHistory { get; set; } = default!;

    #endregion email

    #region others

    public DbSet<UserDeviceToken> UserDeviceTokens { get; set; } = default!;
    public DbSet<UserNotification> UserNotifications { get; set; } = default!;
    public DbSet<UserStatistic> UserStatistics { get; set; } = default!;
    public DbSet<FailedJob> FailedJobs { get; set; } = default!;
    public DbSet<QrCodeScanAudit> QrCodeScanAudits { get; set; } = default!;
    public DbSet<QrToken> QrTokens { get; set; } = default!;
    public DbSet<TenantSetting> TenantSettings { get; set; } = default!;
    public DbSet<Tenant> Tenants { get; set; } = default!;
    public DbSet<TenantApplication> TenantApplications { get; set; } = default!;
    public DbSet<TenantSetupToken> TenantSetupTokens { get; set; } = default!;
    public DbSet<TenantUserSetting> TenantUserSettings { get; set; } = default!;
    public DbSet<PartnerInquiry> PartnerInquiries { get; set; } = default!;
    public DbSet<QueuedJob> QueuedJobs { get; set; } = default!;

    #endregion others

    public T AcquireRepository<T>()
    {
        return containerService.Resolve<T>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantEntries = ChangeTracker
            .Entries<TenantEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (tenantEntries.Count == 0)
            return await base.SaveChangesAsync(cancellationToken);

        var systemContextAccessor = this.containerService.Resolve<ISystemUserContextAccessor>();
        var userContext = systemContextAccessor.Current;

        // Check if the user context is anonymous
        if (userContext is AnonymousUserContext)
        {
            // Fallback to resolve IUserContext from the container (e.g., SystemUserContext or regular user context)
            userContext = this.containerService.Resolve<IUserContext>();
        }

        if (!userContext.IsSystem && string.IsNullOrWhiteSpace(userContext.TenantId))
        {
            throw new InvalidOperationException("TenantId is required for non-system operations");
        }

        foreach (var entry in tenantEntries)
        {
            if (entry.State == EntityState.Added && !string.IsNullOrWhiteSpace(userContext.TenantId))
                entry.Entity.TenantId = userContext.TenantId;
        }

        if (!string.IsNullOrWhiteSpace(userContext.TenantId))
        {
            var tenantId = userContext.TenantId.Replace("'", "''");
            // Current is intentionally one-shot. Restore it while the EF
            // connection is opened so the connection interceptor can apply
            // the same tenant session variable, then clear it again.
            systemContextAccessor.Set(userContext);

            // The connection can be closed and reopened mid-SaveChanges (e.g. the
            // ExecuteSqlRawAsync below runs on its own connection lifetime), and
            // TenantDbConnectionInterceptor prefers HttpContext.Items["TenantId"]
            // (route/header) over the accessor above. If a stale/unrelated tenant
            // header is present on the request (e.g. a superadmin logging in with
            // a leftover X-Tenant-Id from browsing another tenant), the interceptor
            // would re-apply that tenant to the session on reopen, while the row
            // itself carries userContext.TenantId - causing an RLS mismatch. Align
            // the two for the duration of this call, then restore.
            var httpContext = HttpContextAccessor.HttpContext;
            var previousItemsTenantId = httpContext?.Items["TenantId"];
            if (httpContext is not null)
                httpContext.Items["TenantId"] = userContext.TenantId;

            try
            {
                await Database.ExecuteSqlRawAsync($"SET app.tenant_id = '{tenantId}'", cancellationToken);
                return await base.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                systemContextAccessor.Set(AnonymousUserContext.Instance);
                if (httpContext is not null)
                    httpContext.Items["TenantId"] = previousItemsTenantId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

        // Defense in depth: tenant entities are filtered in EF even when the
        // database connection is accidentally configured with a privileged role.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(x => typeof(TenantEntity).IsAssignableFrom(x.ClrType)
                         && x.ClrType != typeof(UniversalChallenge)
                         && x.ClrType != typeof(EmailTemplate)
                         && x.ClrType != typeof(QueuedJob)))
        {
            var method = typeof(TenantDbContext)
                .GetMethod(nameof(ApplyTenantQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);
            method.Invoke(this, [modelBuilder]);
        }

        modelBuilder.Entity<UserChallengePeriodPerformance>()
            .HasIndex(x => new { x.UserId, x.Id })
            .HasDatabaseName("IX_Unique_User_Per_Active_Period")
            .IsUnique();

        modelBuilder.Entity<UserChallengePeriodPerformance>()
            .HasAnnotation("Relational:IndexFilter", "\"IsPeriodActive\" = true");

        modelBuilder.Entity<TenantSetupToken>()
            .HasIndex(x => x.TokenHash)
            .IsUnique();
    }

    void ApplyTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : TenantEntity
    {
        // Deliberately just `entity.TenantId == CurrentTenantId`, not guarded by
        // `!string.IsNullOrWhiteSpace(CurrentTenantId) && ...`. That extra method-call
        // conjunction let EF Core's query compiler constant-fold the whole tenant
        // branch to `false` (baking in whichever ambient CurrentTenantId happened to
        // be active - often none - the first time a given entity's query shape was
        // compiled) and cache that poisoned plan for the process's lifetime, so the
        // row-fetch would keep silently returning nothing for every tenant afterward.
        // A bare `entity.TenantId == CurrentTenantId` doesn't need the guard: SQL
        // NULL comparison already makes it false whenever CurrentTenantId is null.
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => IsSystemContext || entity.TenantId == CurrentTenantId);
    }

}
