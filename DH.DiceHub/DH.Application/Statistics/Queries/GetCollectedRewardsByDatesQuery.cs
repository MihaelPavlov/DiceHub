using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetCollectedRewardsByDatesQuery(string FromDate, string ToDate) : IRequest<OperationResult<List<GetCollectedRewardByDatesModel>>>;

internal class GetCollectedRewardsByDatesQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetCollectedRewardsByDatesQuery, OperationResult<List<GetCollectedRewardByDatesModel>>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<List<GetCollectedRewardByDatesModel>>> Handle(GetCollectedRewardsByDatesQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetCollectedRewardsByDates(request.FromDate, request.ToDate, cancellationToken);
    }
}
