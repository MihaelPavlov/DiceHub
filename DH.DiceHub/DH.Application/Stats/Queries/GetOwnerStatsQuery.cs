using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.SpaceManagementModels.Queries;
using MediatR;

namespace DH.Application.Stats.Queries;

public record GetOwnerStatsQuery : IRequest<GetOwnerStatsQueryModel>;

internal class GetOwnerStatsQueryHandler(
    IStatisticsService statisticsService) : IRequestHandler<GetOwnerStatsQuery, GetOwnerStatsQueryModel>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<GetOwnerStatsQueryModel> Handle(GetOwnerStatsQuery request, CancellationToken cancellationToken)
    {
        return await this.statisticsService.GetOwnerProfileStats(cancellationToken);
    }
}
