using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Enums;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetGameActivityChartDataQuery(ChartGameActivityType Type, DateTime? RangeStart, DateTime? RangeEnd) : IRequest<OperationResult<GetGameActivityChartData>>;

internal  class GetGameActivityChartDataQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetGameActivityChartDataQuery, OperationResult<GetGameActivityChartData>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetGameActivityChartData>> Handle(GetGameActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        return await statisticsService.GetGameActivitydData(request.Type,
            request.RangeStart, request.RangeEnd, cancellationToken);
    }
}