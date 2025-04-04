﻿using DH.Database.Connector;
using DH.Statistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DH.Statistics.Data;

public class StatisticsDbContext : TenantDbContext
{
    public StatisticsDbContext(DbContextOptions options, Assembly entityAssembly) : base(options, entityAssembly)
    {
    }

    public DbSet<ClubVisitorLog> ClubVisitorLogs { get; set; } = default!;
    public DbSet<EventAttendanceLog> EventAttendanceLogs { get; set; } = default!;
    public DbSet<ReservationOutcomeLog> ReservationOutcomeLogs { get; set; } = default!;
    public DbSet<RewardHistoryLog> RewardHistoryLogs { get; set; } = default!;
    public DbSet<ChallengeHistoryLog> ChallengeHistoryLogs { get; set; } = default!;
}
