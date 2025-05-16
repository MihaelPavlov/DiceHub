using Azure.Core;
using DH.Domain.Adapters.Statistics.Enums;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Exceptions;
using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using static DH.Domain.Adapters.Statistics.StatisticJobQueue;

namespace DH.Adapter.Data.Services;

internal class StatisticsService(
    IDbContextFactory<TenantDbContext> dbContextFactory) : IStatisticsService
{
    readonly IDbContextFactory<TenantDbContext> dbContextFactory = dbContextFactory;

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

    public async Task<OperationResult<GetActivityChartData>> GetActivityChartData(ChartActivityType type, string rangeStart, string? rangeEnd, CancellationToken cancellationToken)
    {
        var isRangeStartParsed = DateTime.TryParse(rangeStart, out var parsedRangeStartUtc);

        if (!isRangeStartParsed)
            return new OperationResult<GetActivityChartData>().ReturnWithBadRequestException("Start Date is Missing or Incorrect");

        var rangeStartUtc = parsedRangeStartUtc.ToUniversalTime();
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
                if (string.IsNullOrEmpty(rangeEnd) || !DateTime.TryParse(rangeEnd, out var parsedRangeEndDate))
                    return new OperationResult<GetActivityChartData>().ReturnWithBadRequestException("End Date is Missing or Incorrect");

                rangeEndUtc = parsedRangeEndDate.ToUniversalTime();
                query = query.Where(x => x.LogDate.Date >= rangeStartUtc.Date && x.LogDate <= rangeEndUtc.Value.Date);
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
            while (currentDate.Date <= endDate.Date)
            {
                var log = logs.FirstOrDefault(l => l.Date == currentDate.Date);
                if (log == null)
                {
                    completeLogs.Add(new ActivityLog { UserCount = 0, Date = currentDate.Date });
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

        var startDateUtc= startDate.ToUniversalTime();
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
            var rewards = await context.RewardHistoryLogs
                .Where(x =>
                    x.IsCollected &&
                    x.CollectedDate != null &&
                    x.CollectedDate.Value.Date >= fromDateUtc!.Value.Date &&
                    x.CollectedDate.Value.Date <= toDateUtc!.Value.Date)
                .GroupBy(x => x.RewardId)
                .Select(x => new GetCollectedRewardByDatesModel
                {
                    RewardId = x.Key,
                    CollectedCount = x.Count(),
                })
                .ToListAsync(cancellationToken);

            return new OperationResult<List<GetCollectedRewardByDatesModel>>(rewards);
        }
    }

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
            var collectedRewards = await context.RewardHistoryLogs
                  .Where(x => x.IsCollected && x.CollectedDate != null && x.CollectedDate.Value.Year == year)
                  .GroupBy(x => new { x.CollectedDate!.Value.Year, x.CollectedDate.Value.Month })
                  .Select(g => new RewardsStats
                  {
                      Month = g.Key.Month,
                      CountRewards = g.Count()
                  })
                  .ToListAsync(cancellationToken);

            var expiredRewards = await context.RewardHistoryLogs
                .Where(x => x.IsExpired && x.ExpiredDate != null && x.ExpiredDate.Value.Year == year)
                .GroupBy(x => new { x.ExpiredDate!.Value.Year, x.ExpiredDate.Value.Month })
                .Select(g => new RewardsStats
                {
                    Month = g.Key.Month,
                    CountRewards = g.Count()
                })
                .ToListAsync(cancellationToken);

            var allMonths = Enumerable.Range(1, 12).Select(month => new RewardsStats
            {
                Month = month,
                CountRewards = 0
            }).ToList();

            var completeCollectedRewards = allMonths
              .GroupJoin(collectedRewards, m => m.Month, r => r.Month, (m, r) => new { Month = m.Month, CountRewards = r.Sum(x => x.CountRewards) })
              .Select(grp => new RewardsStats { Month = grp.Month, CountRewards = grp.CountRewards })
              .ToList();

            var completeExpiredRewards = allMonths
                .GroupJoin(expiredRewards, m => m.Month, r => r.Month, (m, r) => new { Month = m.Month, CountRewards = r.Sum(x => x.CountRewards) })
                .Select(grp => new RewardsStats { Month = grp.Month, CountRewards = grp.CountRewards })
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
}
