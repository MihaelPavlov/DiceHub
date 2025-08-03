using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Enums;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetActivityChartDataQuery(ChartActivityType Type, DateTime RangeStart, DateTime? RangeEnd) : IRequest<OperationResult<GetActivityChartData>>;

internal class GetActivityChartDataQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetActivityChartDataQuery, OperationResult<GetActivityChartData>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetActivityChartData>> Handle(GetActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        return await statisticsService.GetActivityChartData(request.Type, request.RangeStart, request.RangeEnd, cancellationToken);
    }
}