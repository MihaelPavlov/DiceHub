using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUniversalChallengeListQuery : IRequest<List<GetUniversalChallengeListQueryModel>>;

internal class GetUniversalChallengeListQueryHandler(IRepository<UniversalChallenge> repository) : IRequestHandler<GetUniversalChallengeListQuery, List<GetUniversalChallengeListQueryModel>>
{
    readonly IRepository<UniversalChallenge> repository = repository;

    public async Task<List<GetUniversalChallengeListQueryModel>> Handle(GetUniversalChallengeListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync(
            x => new GetUniversalChallengeListQueryModel
            {
                Id = x.Id,
                RewardPoints = x.RewardPoints,
                Attempts = x.Attempts,
                MinValue = x.MinValue,
                Description_BG = x.Description_BG,
                Description_EN = x.Description_EN,
                Name_EN = x.Name_EN,
                Name_BG = x.Name_BG,
                Type = x.Type
            }, cancellationToken);
    }
}