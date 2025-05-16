using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetEventAttendanceByIdsQuery(int[] EventIds) : IRequest<OperationResult<GetEventAttendanceChartData>>;

internal class GetEventAttendanceByIdsQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetEventAttendanceByIdsQuery, OperationResult<GetEventAttendanceChartData>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetEventAttendanceChartData>> Handle(GetEventAttendanceByIdsQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetEventAttendanceByIds(request.EventIds, cancellationToken);
    }
}