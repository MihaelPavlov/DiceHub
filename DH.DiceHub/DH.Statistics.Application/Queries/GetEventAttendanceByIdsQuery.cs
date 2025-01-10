using DH.OperationResultCore.Utility;
using DH.Statistics.Data;
using DH.Statistics.Domain.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DH.Statistics.Application.Queries;

public record GetEventAttendanceByIdsQuery(int[] EventIds) : IRequest<OperationResult<GetEventAttendanceChartData>>;

internal class GetEventAttendanceByIdsQueryHandler(IDbContextFactory<StatisticsDbContext> dbContextFactory) : IRequestHandler<GetEventAttendanceByIdsQuery, OperationResult<GetEventAttendanceChartData>>
{
    readonly IDbContextFactory<StatisticsDbContext> dbContextFactory = dbContextFactory;

    public async Task<OperationResult<GetEventAttendanceChartData>> Handle(GetEventAttendanceByIdsQuery request, CancellationToken cancellationToken)
    {
        if (request.EventIds.Length == 0)
            return new OperationResult<GetEventAttendanceChartData>();

        using (var context = await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var events = await context.EventAttendanceLogs
               .Where(x => request.EventIds.Contains(x.EventId))
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