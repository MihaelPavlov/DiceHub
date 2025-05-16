using DH.Domain.Adapters.Statistics.Enums;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;

namespace DH.Application.Statistics.Queries;

public record GetChallengeHistoryLogQuery(ChallengeHistoryLogType Type) : IRequest<OperationResult<List<GetChallengeHistoryLogQueryResponse>>>;

internal class GetChallengeHistoryLogQueryHandler(IStatisticsService statisticsService) : IRequestHandler<GetChallengeHistoryLogQuery, OperationResult<List<GetChallengeHistoryLogQueryResponse>>>
{
    readonly IStatisticsService statisticsService = statisticsService;

    public async Task<OperationResult<List<GetChallengeHistoryLogQueryResponse>>> Handle(GetChallengeHistoryLogQuery request, CancellationToken cancellationToken)
    {
        return await statisticsService.GetChallengeHistoryLogs(request.Type, cancellationToken);
    }
}
