using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetChallengeHistoryLogQuery(ChallengeHistoryLogType Type) : IRequest<OperationResult<List<GetChallengeHistoryLogQueryResponse>>>;

internal class GetChallengeHistoryLogQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetChallengeHistoryLogQuery, OperationResult<List<GetChallengeHistoryLogQueryResponse>>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<List<GetChallengeHistoryLogQueryResponse>>> Handle(GetChallengeHistoryLogQuery request, CancellationToken cancellationToken)
    {
        DateTime startDate, endDate;

        switch (request.Type)
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

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var challenges = await context.ChallengeHistoryLogs
                .Where(x => x.Outcome == ChallengeOutcome.Completed && x.OutcomeDate >= startDate && x.OutcomeDate <= endDate)
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
}

public class GetChallengeHistoryLogQueryResponse
{
    public string UserId { get; set; } = string.Empty;
    public int ChallengeCount { get; set; }
}

public enum ChallengeHistoryLogType
{
    Weekly = 0,
    Monthly = 1,
    Yearly = 2,
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}