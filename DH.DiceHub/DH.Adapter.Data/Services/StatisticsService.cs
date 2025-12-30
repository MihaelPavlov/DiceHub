using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Enums;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Exceptions;
using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

internal class StatisticsService(
    IDbContextFactory<TenantDbContext> dbContextFactory,
    IUserContext userContext) : IStatisticsService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory = dbContextFactory;
    readonly IUserContext userContext = userContext;

    public async Task<GetUserStatsQueryModel> GetUserProfileStats(CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var tables = await context.SpaceTables
                .Where(r => r.CreatedBy == this.userContext.UserId)
                .ToListAsync(cancellationToken);

            var participantInTables = await context.SpaceTableParticipants
                .Include(x => x.SpaceTable)
                .Where(r => r.UserId == this.userContext.UserId)
                .ToListAsync(cancellationToken);

            var uniqueGamesPlayed = tables
                .Select(x => x.GameId)
                .Union(participantInTables
                    .Select(x => x.SpaceTable.GameId))
                .Distinct()
                .Count();

            var gameReservationsCount = await context.GameReservations
                .CountAsync(x => x.IsReservationSuccessful && x.UserId == this.userContext.UserId, cancellationToken);

            var tableReservationsCount = await context.SpaceTableReservations
                .CountAsync(x => x.IsReservationSuccessful && x.UserId == this.userContext.UserId, cancellationToken);

            var events = await context.EventParticipants.Where(
                x => x.UserId == this.userContext.UserId).ToListAsync(cancellationToken);

            return new GetUserStatsQueryModel(uniqueGamesPlayed, gameReservationsCount + tableReservationsCount, events.Count);
        }
    }

    public async Task<GetOwnerStatsQueryModel> GetOwnerProfileStats(CancellationToken cancellationToken)
    {
        using (var context = await this.dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var uniqueGames = await context.Games.CountAsync(cancellationToken);
            var uniqueEvents = await context.Events.CountAsync(cancellationToken);

            var gameReservationsCount = await context.GameReservations
                .CountAsync(x => x.IsReservationSuccessful, cancellationToken);

            var tableReservationsCount = await context.SpaceTableReservations
                .CountAsync(x => x.IsReservationSuccessful, cancellationToken);

            return new GetOwnerStatsQueryModel(uniqueGames, gameReservationsCount + tableReservationsCount, uniqueEvents);
        }
    }

    public async Task ChallengeProcessingOutcomeMessage(ChallengeProcessingOutcomeJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            await context.ChallengeHistoryLogs.AddAsync(new ChallengeHistoryLog
            {
                UserId = job.UserId,
                OutcomeDate = job.OutcomeDate,
                ChallengeId = job.ChallengeId,
                Outcome = job.Outcome,
                CreatedDate = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task ClubActivityDetectedMessage(ClubActivityDetectedJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            await context.ClubVisitorLogs.AddAsync(new ClubVisitorLog
            {
                UserId = job.UserId,
                LogDate = job.LogDate,
                CreatedDate = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task EventAttendanceDetectedMessage(EventAttendanceDetectedJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            if (job.Type == AttendanceAction.Joining)
            {
                await context.EventAttendanceLogs.AddAsync(new EventAttendanceLog
                {
                    UserId = job.UserId,
                    LogDate = job.LogDate,
                    EventId = job.EventId,
                    CreatedDate = DateTime.UtcNow,
                });
            }
            else if (job.Type == AttendanceAction.Leaving)
            {
                var log = await context.EventAttendanceLogs
                    .FirstOrDefaultAsync(x =>
                        x.UserId == job.UserId &&
                        x.EventId == job.EventId
                    ) ?? throw new NotFoundException(
                        $"EventAttendanceLogs with UserId {job.UserId} and EventId {job.EventId} was not found");

                context.EventAttendanceLogs.Remove(log);
            }
            await context.SaveChangesAsync();
        }
    }

    public async Task ReservationProcessingOutcomeMessage(ReservationProcessingOutcomeJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            await context.ReservationOutcomeLogs.AddAsync(new ReservationOutcomeLog
            {
                UserId = job.UserId,
                ReservationId = job.ReservationId,
                Outcome = job.Outcome,
                Type = job.Type,
                OutcomeDate = job.OutcomeDate,
                CreatedDate = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task RewardActionDetectedMessage(RewardActionDetectedJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            await context.RewardHistoryLogs.AddAsync(new RewardHistoryLog
            {
                UserId = job.UserId,
                RewardId = job.RewardId,
                CollectedDate = job.CollectedDate,
                ExpiredDate = job.ExpiredDate,
                IsCollected = job.IsCollected,
                IsExpired = job.IsExpired,
                CreatedDate = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task GameEngagementDetectedMessage(GameEngagementDetectedJob job)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync())
        {
            await context.GameEngagementLogs.AddAsync(new GameEngagementLog
            {
                UserId = job.UserId,
                GameId = job.GameId,
                DetectedOn = job.DetectedOn,
                CreatedDate = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task<OperationResult<GetActivityChartData>> GetActivityChartData(ChartActivityType type, DateTime rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken)
    {
        var rangeStartUtc = rangeStart;
        DateTime? rangeEndUtc = null;

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            IQueryable<ClubVisitorLog> query = context.ClubVisitorLogs;

            if (type == ChartActivityType.Monthly)
            {
                query = query.Where(x => x.LogDate.Year == rangeStartUtc.Year && x.LogDate.Month == rangeStartUtc.Month);
            }
            else if (type == ChartActivityType.Yearly)
            {
                query = query.Where(x => x.LogDate.Year == rangeStartUtc.Year);
            }
            else
            {
                if (rangeEnd == null)
                    return new OperationResult<GetActivityChartData>().ReturnWithBadRequestException("End Date is Missing");

                rangeEndUtc = rangeEnd;
                query = query.Where(x => x.LogDate >= rangeStartUtc && x.LogDate <= rangeEndUtc.Value);
            }

            var logs = await query.ToListAsync(cancellationToken);
            var groupedLogs = logs
                .GroupBy(log => GetGroupKey(log.LogDate, type))
                .Select(group => new ActivityLog
                {
                    UserCount = group.Count(),
                    Date = group.Key
                })
            .ToList();

            var completeLogs = FillMissingPeriods(groupedLogs, rangeStartUtc, rangeEndUtc, type);

            return new OperationResult<GetActivityChartData>(new GetActivityChartData { Logs = completeLogs });
        }
    }

    private DateTime GetGroupKey(DateTime date, ChartActivityType type)
    {
        return type switch
        {
            ChartActivityType.Weekly => date.Date, // Group by each day
            ChartActivityType.Monthly => date.Date, // Group by each day
            ChartActivityType.Yearly => new DateTime(date.Year, date.Month, 1), // Group by each month
            _ => date.Date
        };
    }

    private List<ActivityLog> FillMissingPeriods(List<ActivityLog> logs, DateTime rangeStart, DateTime? rangeEnd, ChartActivityType type)
    {
        var completeLogs = new List<ActivityLog>();
        var currentDate = rangeStart;

        if (type == ChartActivityType.Weekly)
        {
            var endDate = rangeEnd ?? rangeStart;
            while (currentDate <= endDate)
            {
                var log = logs.FirstOrDefault(l => l.Date == currentDate.Date);
                if (log == null)
                {
                    completeLogs.Add(new ActivityLog { UserCount = 0, Date = currentDate });
                }
                else
                {
                    completeLogs.Add(log);
                }
                currentDate = currentDate.AddDays(1);
            }
        }
        else if (type == ChartActivityType.Monthly)
        {
            var daysInMonth = DateTime.DaysInMonth(rangeStart.Year, rangeStart.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(rangeStart.Year, rangeStart.Month, day);
                var log = logs.FirstOrDefault(l => l.Date == date);
                if (log == null)
                {
                    completeLogs.Add(new ActivityLog { UserCount = 0, Date = date });
                }
                else
                {
                    completeLogs.Add(log);
                }
            }
        }
        else if (type == ChartActivityType.Yearly)
        {
            while (currentDate.Year == rangeStart.Year)
            {
                var log = logs.FirstOrDefault(l => l.Date.Year == currentDate.Year && l.Date.Month == currentDate.Month);
                if (log == null)
                {
                    completeLogs.Add(new ActivityLog { UserCount = 0, Date = new DateTime(currentDate.Year, currentDate.Month, 1) });
                }
                else
                {
                    completeLogs.Add(log);
                }
                currentDate = currentDate.AddMonths(1);
            }
        }

        return completeLogs;
    }

    public async Task<OperationResult<List<GetChallengeHistoryLogQueryResponse>>> GetChallengeHistoryLogs(ChallengeHistoryLogType type, CancellationToken cancellationToken)
    {
        DateTime startDate, endDate;

        switch (type)
        {
            case ChallengeHistoryLogType.Weekly:
                startDate = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);
                endDate = startDate.AddDays(7).AddTicks(-1);
                break;
            case ChallengeHistoryLogType.Monthly:
                startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                break;
            case ChallengeHistoryLogType.Yearly:
                startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                endDate = startDate.AddYears(1).AddTicks(-1);
                break;
            default:
                return new OperationResult<List<GetChallengeHistoryLogQueryResponse>>()
                    .ReturnWithBadRequestException("Challenge History Log Type was not correct");
        }

        var startDateUtc = startDate.ToUniversalTime();
        var endDateUtc = endDate.ToUniversalTime();

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var challenges = await context.ChallengeHistoryLogs
                .Where(x => x.Outcome == ChallengeOutcome.Completed && x.OutcomeDate >= startDateUtc && x.OutcomeDate <= endDateUtc)
                .GroupBy(x => x.UserId)
                .Select(x => new GetChallengeHistoryLogQueryResponse
                {
                    UserId = x.Key,
                    ChallengeCount = x.Count()
                })
                .OrderByDescending(x => x.ChallengeCount)
                .ToListAsync(cancellationToken);

            return new OperationResult<List<GetChallengeHistoryLogQueryResponse>>(challenges);
        }
    }

    public async Task<OperationResult<List<GetCollectedRewardByDatesModel>>> GetCollectedRewardsByDates(string fromDate, string toDate, CancellationToken cancellationToken)
    {
        var (fromDateUtc, toDateUtc, errorMessage) = DateValidator.ValidateAndParseDates(fromDate, toDate);

        if (errorMessage != null)
            return new OperationResult<List<GetCollectedRewardByDatesModel>>()
                .ReturnWithBadRequestException(errorMessage);


        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var systemRewards = await context.ChallengeRewards
                .Select(x => new Test(x.Id, x.CashEquivalent))
                .ToListAsync(cancellationToken);

            var groupedRewards = await context.RewardHistoryLogs
                .Where(x =>
                    x.IsCollected &&
                    x.CollectedDate != null &&
                    x.CollectedDate.Value.Date >= fromDateUtc!.Value.Date &&
                    x.CollectedDate.Value.Date <= toDateUtc!.Value.Date)
                .GroupBy(x => x.RewardId)
                .ToListAsync(cancellationToken);

            var rewards = groupedRewards
                .Select(group =>
                {
                    var count = group.Count();
                    var rewardId = group.Key;

                    var reward = systemRewards.FirstOrDefault(sr => sr.Id == rewardId);
                    var cashEquivalent = reward?.CashEquivalent ?? 0;

                    return new GetCollectedRewardByDatesModel
                    {
                        RewardId = rewardId,
                        CollectedCount = count,
                        TotalCashEquivalent = cashEquivalent * count
                    };
                })
                .ToList();
            return new OperationResult<List<GetCollectedRewardByDatesModel>>(rewards);
        }
    }
    public record Test(int Id, decimal CashEquivalent);

    public async Task<OperationResult<GetEventAttendanceChartData>> GetEventAttendanceByIds(int[] eventIds, CancellationToken cancellationToken)
    {
        if (eventIds.Length == 0)
            return new OperationResult<GetEventAttendanceChartData>();

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var events = await context.EventAttendanceLogs
               .Where(x => eventIds.Contains(x.EventId))
               .GroupBy(x => x.EventId)
               .Select(x => new EventAttendance
               {
                   EventId = x.Key,
                   UserAttendedCount = x.Count(),
               })
               .ToListAsync(cancellationToken);

            return new OperationResult<GetEventAttendanceChartData>(new GetEventAttendanceChartData { EventAttendances = events });
        }
    }

    public async Task<OperationResult<GetEventAttendanceChartData>> GetEventAttendanceChartData(string fromDate, string toDate, CancellationToken cancellationToken)
    {
        var (fromDateUtc, toDateUtc, errorMessage) = DateValidator.ValidateAndParseDates(fromDate, toDate);

        if (errorMessage != null)
            return new OperationResult<GetEventAttendanceChartData>()
                .ReturnWithBadRequestException(errorMessage);

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var events = await context.EventAttendanceLogs
                .Where(x =>
                    x.LogDate.Date >= fromDateUtc!.Value.Date &&
                    x.LogDate.Date <= toDateUtc!.Value.Date)
                .GroupBy(x => x.EventId)
                .Select(x => new EventAttendance
                {
                    EventId = x.Key,
                    UserAttendedCount = x.Count(),
                })
                .ToListAsync(cancellationToken);

            return new OperationResult<GetEventAttendanceChartData>(new GetEventAttendanceChartData { EventAttendances = events });
        }
    }

    public async Task<OperationResult<GetExpiredCollectedRewardsChartDataModel>> GetExpiredCollectedRewardsChartData(int year, CancellationToken cancellationToken)
    {
        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var allMonths = Enumerable.Range(1, 12)
             .Select(month => new RewardsStats
             {
                 Month = month,
                 CountRewards = 0,
                 TotalCashEquivalent = 0m
             })
             .ToList();

            var collectedRewards = await context.RewardHistoryLogs
                .Where(x => x.IsCollected && x.CollectedDate != null && x.CollectedDate.Value.Year == year)
                .Join(context.ChallengeRewards,
                    rhl => rhl.RewardId,
                    cr => cr.Id,
                    (rhl, cr) => new { rhl.CollectedDate, cr.CashEquivalent })
                .GroupBy(x => x.CollectedDate!.Value.Month)
                .Select(g => new RewardsStats
                {
                    Month = g.Key,
                    CountRewards = g.Count(),
                    TotalCashEquivalent = g.Sum(x => x.CashEquivalent)
                })
                .ToListAsync(cancellationToken);

            var expiredRewards = await context.RewardHistoryLogs
                .Where(x => x.IsExpired && x.ExpiredDate != null && x.ExpiredDate.Value.Year == year)
                .Join(context.ChallengeRewards,
                    rhl => rhl.RewardId,
                    cr => cr.Id,
                    (rhl, cr) => new { rhl.ExpiredDate, cr.CashEquivalent })
                .GroupBy(x => x.ExpiredDate!.Value.Month)
                .Select(g => new RewardsStats
                {
                    Month = g.Key,
                    CountRewards = g.Count(),
                    TotalCashEquivalent = g.Sum(x => x.CashEquivalent)
                })
                .ToListAsync(cancellationToken);

            var completeCollectedRewards = allMonths
              .GroupJoin(collectedRewards,
                m => m.Month,
                r => r.Month,
                (m, r) => r.DefaultIfEmpty(new RewardsStats { Month = m.Month, CountRewards = 0, TotalCashEquivalent = 0m }))
              .SelectMany(grp => grp)
              .OrderBy(x => x.Month)
              .ToList();

            var completeExpiredRewards = allMonths
              .GroupJoin(expiredRewards,
                m => m.Month,
                r => r.Month,
                (m, r) => r.DefaultIfEmpty(new RewardsStats { Month = m.Month, CountRewards = 0, TotalCashEquivalent = 0m }))
              .SelectMany(grp => grp)
              .OrderBy(x => x.Month)
              .ToList();

            var result = new GetExpiredCollectedRewardsChartDataModel
            {
                Collected = completeCollectedRewards,
                Expired = completeExpiredRewards
            };

            return new OperationResult<GetExpiredCollectedRewardsChartDataModel>(result);
        }
    }

    public async Task<OperationResult<GetReservationChartData>> GetReservationChartData(string fromDate, string toDate, CancellationToken cancellationToken)
    {
        var (fromDateUtc, toDateUtc, errorMessage) = DateValidator.ValidateAndParseDates(fromDate, toDate);

        if (errorMessage != null)
            return new OperationResult<GetReservationChartData>()
                .ReturnWithBadRequestException(errorMessage);

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var reservations = await context.ReservationOutcomeLogs
                .Where(x =>
                    x.OutcomeDate.Date >= fromDateUtc!.Value.Date &&
                    x.OutcomeDate.Date <= toDateUtc!.Value.Date)
                .ToListAsync(cancellationToken);

            var gameReservations = reservations.Where(r => r.Type == ReservationType.Game)
                .GroupBy(r => r.Outcome)
                .Select(g => new
                {
                    Outcome = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(g => g.Outcome, g => g.Count);

            var tableReservations = reservations.Where(r => r.Type == ReservationType.Table)
                .GroupBy(r => r.Outcome)
                .Select(g => new
                {
                    Outcome = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(g => g.Outcome, g => g.Count);

            var gameStats = new ReservationStats
            {
                Completed = gameReservations.ContainsKey(ReservationOutcome.Completed) ? gameReservations[ReservationOutcome.Completed] : 0,
                Cancelled = gameReservations.ContainsKey(ReservationOutcome.Cancelled) ? gameReservations[ReservationOutcome.Cancelled] : 0
            };

            var tableStats = new ReservationStats
            {
                Completed = tableReservations.ContainsKey(ReservationOutcome.Completed) ? tableReservations[ReservationOutcome.Completed] : 0,
                Cancelled = tableReservations.ContainsKey(ReservationOutcome.Cancelled) ? tableReservations[ReservationOutcome.Cancelled] : 0
            };

            var result = new GetReservationChartData
            {
                GameReservationStats = gameStats,
                TableReservationStats = tableStats
            };

            return new OperationResult<GetReservationChartData>(result);
        }
    }

    public async Task<OperationResult<GetGameActivityChartData>> GetGameActivitydData(ChartGameActivityType type, DateTime? rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken)
    {
        DateTime? rangeStartUtc = null;
        DateTime? rangeEndUtc = null;

        // Only parse rangeStart if type is not AllTime
        if (type != ChartGameActivityType.AlTime)
        {
            if (rangeStart == null)
                return new OperationResult<GetGameActivityChartData>()
                    .ReturnWithBadRequestException("Start Date is Missing");

            rangeStartUtc = rangeStart;
        }

        if (type == ChartGameActivityType.Weekly)
        {
            if (rangeEnd == null)
                return new OperationResult<GetGameActivityChartData>()
                    .ReturnWithBadRequestException("End Date is Missing");

            rangeEndUtc = rangeEnd;
        }

        using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var logs = await context.GameEngagementLogs.ToListAsync(cancellationToken);
        var stats = await (
            from log in context.GameEngagementLogs
            join game in context.Games on log.GameId equals game.Id
            where
            (type == ChartGameActivityType.Monthly &&
                rangeStartUtc != null &&
                log.DetectedOn.Year == rangeStartUtc.Value.Year &&
                log.DetectedOn.Month == rangeStartUtc.Value.Month)
            ||
            (type == ChartGameActivityType.Yearly &&
                rangeStartUtc != null &&
                log.DetectedOn.Year == rangeStartUtc.Value.Year)
            ||
            (type == ChartGameActivityType.Weekly &&
                rangeStartUtc != null && rangeEndUtc != null &&
                log.DetectedOn >= rangeStartUtc.Value &&
                log.DetectedOn <= rangeEndUtc.Value)
            ||
            (type == ChartGameActivityType.AlTime)
            group log by new { game.Id, game.Name, ImageUrl = game.ImageUrl } into g
            select new GameActivityStats
            {
                GameId = g.Key.Id,
                GameName = g.Key.Name,
                TimesPlayed = g.Count(),
                GameImageUrl = g.Key.ImageUrl
            }
        )
        .OrderByDescending(x => x.TimesPlayed)
        .ToListAsync(cancellationToken);

        return new OperationResult<GetGameActivityChartData>(
            new GetGameActivityChartData { Games = stats }
        );
    }

    public async Task<OperationResult<GetUsersWhoPlayedGameData>> GetGameActivityUsersData(int gameId, ChartGameActivityType type, DateTime? rangeStart, DateTime? rangeEnd, CancellationToken cancellationToken)
    {
        DateTime? rangeStartUtc = null;
        DateTime? rangeEndUtc = null;

        // Only parse rangeStart if type is not AllTime
        if (type != ChartGameActivityType.AlTime)
        {
            if (rangeStart == null)
                return new OperationResult<GetUsersWhoPlayedGameData>()
                    .ReturnWithBadRequestException("Start Date is Missing");

            rangeStartUtc = rangeStart;
        }

        if (type == ChartGameActivityType.Weekly)
        {
            if (rangeEnd == null)
                return new OperationResult<GetUsersWhoPlayedGameData>()
                    .ReturnWithBadRequestException("End Date is Missing");

            rangeEndUtc = rangeEnd;
        }

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = from log in context.GameEngagementLogs
                        where log.GameId == gameId
                        select log;

            // Apply filtering conditionally based on chart type
            if (type == ChartGameActivityType.Monthly && rangeStartUtc != null)
            {
                query = query.Where(log =>
                    log.DetectedOn.Year == rangeStartUtc.Value.Year &&
                    log.DetectedOn.Month == rangeStartUtc.Value.Month);
            }
            else if (type == ChartGameActivityType.Yearly && rangeStartUtc != null)
            {
                query = query.Where(log => log.DetectedOn.Year == rangeStartUtc.Value.Year);
            }
            else if (type == ChartGameActivityType.Weekly && rangeStartUtc != null && rangeEndUtc != null)
            {
                query = query.Where(log => log.DetectedOn >= rangeStartUtc.Value &&
                                           log.DetectedOn <= rangeEndUtc.Value);
            }

            var result = await query
                .GroupBy(x => x.UserId)
                .Select(g => new GameUserActivity
                {
                    UserId = g.Key,
                    UserDisplayName = string.Empty,
                    LastPlayedAt = g.Max(x => x.DetectedOn), // last played
                    TimesPlayedFromUser = g.Count()
                })
                .OrderByDescending(x => x.LastPlayedAt)
                .ToListAsync(cancellationToken);

            return new OperationResult<GetUsersWhoPlayedGameData>(
                new GetUsersWhoPlayedGameData { Users = result });
        }
    }
}
