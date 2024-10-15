using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserChallengeListQuery : IRequest<List<GetUserChallengeListQueryModel>>;

internal class GetUserChallengeListQueryHandler : IRequestHandler<GetUserChallengeListQuery, List<GetUserChallengeListQueryModel>>
{
    readonly IChallengeService challengeService;
    readonly IUserContext userContext;

    public GetUserChallengeListQueryHandler(IUserContext userContext, IChallengeService challengeService)
    {
        this.userContext = userContext;
        this.challengeService = challengeService;
    }
    public async Task<List<GetUserChallengeListQueryModel>> Handle(GetUserChallengeListQuery request, CancellationToken cancellationToken)
    {
        return await this.challengeService.GetUserChallenges(cancellationToken);
    }
}
