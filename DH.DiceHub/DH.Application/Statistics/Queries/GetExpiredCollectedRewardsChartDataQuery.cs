using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetExpiredCollectedRewardsChartDataQuery(int Year) : IRequest<OperationResult<GetExpiredCollectedRewardsChartDataModel>>;

internal class GetExpiredCollectedRewardsChartDataQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetExpiredCollectedRewardsChartDataQuery, OperationResult<GetExpiredCollectedRewardsChartDataModel>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<GetExpiredCollectedRewardsChartDataModel>> Handle(GetExpiredCollectedRewardsChartDataQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetExpiredCollectedRewardsChartData(request.Year, cancellationToken);
    }
}