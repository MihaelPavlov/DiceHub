using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler : IRequestHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService;
    readonly IUserContext userContext;
    readonly SynchronizeUsersChallengesQueue synchronizeUsersChallengesQueue;

    public GetGameListQueryHandler(IGameService gameService, IUserContext userContext, SynchronizeUsersChallengesQueue synchronizeUsersChallengesQueue)
    {
        this.gameService = gameService;
        this.userContext = userContext;
        this.synchronizeUsersChallengesQueue = synchronizeUsersChallengesQueue;
    }

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListQuery request, CancellationToken cancellationToken)
    {
        this.synchronizeUsersChallengesQueue.AddSynchronizeNewUserJob(this.userContext.UserId);
        //this.synchronizeUsersChallengesQueue.AddChallengeInitiationJob(this.userContext.UserId, DateTime.UtcNow.AddMinutes(1));

        return await gameService.GetGameListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
