using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetChallengeListWithFilterQuery(int[] GameIds) : IRequest<List<GetChallengeListWithFilterQueryModel>>;

internal class GetChallengeListWithFilterQueryHandler : IRequestHandler<GetChallengeListWithFilterQuery, List<GetChallengeListWithFilterQueryModel>>
{
    readonly IRepository<Challenge> repository;

    public GetChallengeListWithFilterQueryHandler(IRepository<Challenge> repository)
    {
        this.repository = repository;
    }

    public async Task<List<GetChallengeListWithFilterQueryModel>> Handle(GetChallengeListWithFilterQuery request, CancellationToken cancellationToken)
    {
        if (request.GameIds.Count() != 0)
        {
            return await this.repository.GetWithPropertiesAsync(
                x => request.GameIds.Contains(x.GameId),
                x => new GetChallengeListWithFilterQueryModel
                {
                    Id = x.Id,
                    RewardPoints = x.RewardPoints,
                    GameImageUrl = x.Game.ImageUrl,
                    Attempts = x.Attempts,
                }, cancellationToken);
        }
        return await this.repository.GetWithPropertiesAsync(
            x => new GetChallengeListWithFilterQueryModel
            {
                Id = x.Id,
                RewardPoints = x.RewardPoints,
                GameImageUrl = x.Game.ImageUrl,
                Attempts = x.Attempts,
            }, cancellationToken);
    }
}