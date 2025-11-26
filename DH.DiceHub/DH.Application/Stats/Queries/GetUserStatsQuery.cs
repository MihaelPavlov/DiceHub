using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.SpaceManagementModels.Queries;
using MediatR;

namespace DH.Application.Stats.Queries;

public record GetUserStatsQuery : IRequest<GetUserStatsQueryModel>;

internal class GetUserStatsQueryHandler(
    IStatisticsService statisticsService) : IRequestHandler<GetUserStatsQuery, GetUserStatsQueryModel>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<GetUserStatsQueryModel> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetUserProfileStats(cancellationToken);
    }
}
