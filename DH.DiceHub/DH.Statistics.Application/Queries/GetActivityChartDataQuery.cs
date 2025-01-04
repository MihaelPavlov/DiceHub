using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Entities;
using DH.Statistics.Domain.Enums;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetActivityChartDataQuery(ChartActivityType Type, string RangeStart, string? RangeEnd) : IRequest<OperationResult<GetActivityChartData>>;

internal class GetActivityChartDataQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetActivityChartDataQuery, OperationResult<GetActivityChartData>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<GetActivityChartData>> Handle(GetActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        var isRangeStartParsed = DateTime.TryParse(request.RangeStart, out var rangeStartUtc);

        if (!isRangeStartParsed)
            return new OperationResult<GetActivityChartData>().ReturnWithBadRequestException("Start Date is Missing or Incorrect");

        DateTime? rangeEndUtc = null;

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            IQueryable<ClubVisitorLog> query = context.ClubVisitorLogs;

            if (request.Type == ChartActivityType.Monthly)
            {
                query = query.Where(x => x.LogDate.Year == rangeStartUtc.Year && x.LogDate.Month == rangeStartUtc.Month);
            }
            else if (request.Type == ChartActivityType.Yearly)
            {
                query = query.Where(x => x.LogDate.Year == rangeStartUtc.Year);
            }
            else
            {
                if (string.IsNullOrEmpty(request.RangeEnd) || !DateTime.TryParse(request.RangeEnd, out var parsedRangeEndDate))
                    return new OperationResult<GetActivityChartData>().ReturnWithBadRequestException("End Date is Missing or Incorrect");

                rangeEndUtc = parsedRangeEndDate;
                query = query.Where(x => x.LogDate.Date >= rangeStartUtc.Date && x.LogDate <= rangeEndUtc.Value.Date);
            }

            var logs = await query.ToListAsync(cancellationToken);

            var groupedLogs = logs
                .GroupBy(log => GetGroupKey(log.LogDate, request.Type))
                .Select(group => new ActivityLog
                {
                    UserCount = group.Count(),
                    Date = group.Key
                })
                .ToList();

            var completeLogs = FillMissingPeriods(groupedLogs, rangeStartUtc, rangeEndUtc, request.Type);

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
}