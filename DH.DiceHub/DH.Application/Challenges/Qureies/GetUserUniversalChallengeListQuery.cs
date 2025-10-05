using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserUniversalChallengeListQuery : IRequest<List<GetUserUniversalChallengeListQueryModel>>;

internal class GetUserUniversalChallengeListQueryHandler(IChallengeService service) : IRequestHandler<GetUserUniversalChallengeListQuery, List<GetUserUniversalChallengeListQueryModel>>
{
    readonly IChallengeService service = service;

    public async Task<List<GetUserUniversalChallengeListQueryModel>> Handle(GetUserUniversalChallengeListQuery request, CancellationToken cancellationToken)
    {
        return await this.service.GetUserUniversalChallenges(cancellationToken);
    }
}