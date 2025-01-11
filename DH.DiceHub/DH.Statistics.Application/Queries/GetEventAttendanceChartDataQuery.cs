using DH.OperationResultCore.Extension;
using DH.OperationResultCore.Utility;
using DH.Statistics.Application.Helpers;
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
        var (fromDateUtc, toDateUtc, errorMessage) = request.ValidateAndParseDates();

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
}