using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserChallengeListQuery : IRequest<List<GetUserChallengeListQueryModel>>;

internal class GetUserChallengeListQueryHandler : IRequestHandler<GetUserChallengeListQuery, List<GetUserChallengeListQueryModel>>
{
    readonly IRepository<UserChallenge> repository;
    readonly IUserContext userContext;

    public GetUserChallengeListQueryHandler(IRepository<UserChallenge> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }
    public async Task<List<GetUserChallengeListQueryModel>> Handle(GetUserChallengeListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync(
            x => this.userContext.UserId == x.UserId && x.IsActive,
            x => new GetUserChallengeListQueryModel
            {
                Id = x.Id,
                Description = x.Challenge.Description,
                RewardPoints = x.Challenge.RewardPoints,
                Type = x.Challenge.Type,
                GameImageId = x.Challenge.Game.Image.Id,
                GameName = x.Challenge.Game.Name,
                CurrentAttemps = x.AttemptCount,
                MaxAttempts = x.Challenge.Attempts,
            }, cancellationToken);
    }
}
