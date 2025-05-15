using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using System.Reflection;
using DH.Domain;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data;

public class TenantDbContext : DbContext, ITenantDbContext
{
    readonly IContainerService containerService;

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
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DH.DiceHub;User Id=postgres;Password=1qaz!QAZ;");
        }
#endif
    }

    #region games

    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameReview> GameReviews { get; set; } = default!;
    public DbSet<GameLike> GameLikes { get; set; } = default!;
    public DbSet<GameCategory> GameCategories { get; set; } = default!;
    public DbSet<GameImage> GameImages { get; set; } = default!;
    public DbSet<GameReservation> GameReservations { get; set; } = default!;
    public DbSet<GameInventory> GameInventories { get; set; } = default!;

    #endregion games

    #region events

    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<EventImage> EventImages { get; set; } = default!;
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
    public DbSet<ChallengeStatistic> ChallengeStatistics { get; set; } = default!;
    public DbSet<UserChallenge> UserChallenges { get; set; } = default!;
    public DbSet<ChallengeReward> ChallengeRewards { get; set; } = default!;
    public DbSet<ChallengeRewardImage> ChallengeRewardImages { get; set; } = default!;
    public DbSet<UserChallengeReward> UserChallengeRewards { get; set; } = default!;
    public DbSet<UserChallengePeriodReward> UserChallengePeriodRewards { get; set; } = default!;
    public DbSet<UserChallengePeriodPerformance> UserChallengePeriodPerformances { get; set; } = default!;

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
    public DbSet<TenantSetting> TenantSettings { get; set; } = default!;
    public DbSet<TenantUserSetting> TenantUserSettings { get; set; } = default!;
    public DbSet<QueuedJob> QueuedJobs { get; set; } = default!;

    #endregion others

    public T AcquireRepository<T>()
    {
        return containerService.Resolve<T>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ////TODO: Check if we ened to configure the mapping manually or the 24 line is working 
        //CommonMapping.Configure(builder);

        base.OnModelCreating(modelBuilder);
    }
}
