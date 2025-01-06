using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetEventAttendanceChartDataQuery(string FromDate, string ToDate) : IRequest<OperationResult<GetEventAttendanceChartData>>;

internal class GetEventAttendanceChartDataQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetEventAttendanceChartDataQuery, OperationResult<GetEventAttendanceChartData>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<GetEventAttendanceChartData>> Handle(GetEventAttendanceChartDataQuery request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(request.FromDate, out var fromDateUtc))
            return new OperationResult<GetEventAttendanceChartData>().ReturnWithBadRequestException("From Date is Missing or Incorrect");

        if (!DateTime.TryParse(request.ToDate, out var toDateUtc))
            return new OperationResult<GetEventAttendanceChartData>().ReturnWithBadRequestException("To Date is Missing or Incorrect");

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var events = await context.EventAttendanceLogs
                .Where(x => x.LogDate.Date >= fromDateUtc.Date && x.LogDate.Date <= toDateUtc.Date)
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
}