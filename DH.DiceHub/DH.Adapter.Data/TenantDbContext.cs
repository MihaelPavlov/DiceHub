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
            optionsBuilder.UseSqlServer("server=(local); database=DH.DiceHub; Integrated Security=true; encrypt=false");
        }
#endif
    }

    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameReview> GameReviews { get; set; } = default!;
    public DbSet<GameLike> GameLikes { get; set; } = default!;
    public DbSet<GameCategory> GameCategories { get; set; } = default!;
    public DbSet<GameImage> GameImages { get; set; } = default!;
    public DbSet<GameReservation> GameReservations { get; set; } = default!;
    public DbSet<GameInventory> GameInventories { get; set; } = default!;
    public DbSet<GameQrCode> GameQrCodes { get; set; } = default!;
    public DbSet<FailedJob> FailedJobs { get; set; } = default!;
    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<EventImage> EventImages { get; set; } = default!;
    public DbSet<EventParticipant> EventParticipants { get; set; } = default!;
    public DbSet<Room> Rooms { get; set; } = default!;
    public DbSet<RoomParticipant> RoomParticipants { get; set; } = default!;
    public DbSet<RoomMessage> RoomMessages { get; set; } = default!;
    public DbSet<RoomInfoMessage> RoomInfoMessages { get; set; } = default!;

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
