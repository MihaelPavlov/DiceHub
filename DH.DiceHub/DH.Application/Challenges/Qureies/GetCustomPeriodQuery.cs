using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetCustomPeriodQuery : IRequest<GetCustomPeriodQueryModel>;

internal class GetCustomPeriodQueryHandler(
    IRepository<CustomPeriodReward> customPeriodRewardRepository,
    IRepository<CustomPeriodChallenge> customPeriodChallengeRepository) : IRequestHandler<GetCustomPeriodQuery, GetCustomPeriodQueryModel>
{
    readonly IRepository<CustomPeriodReward> customPeriodRewardRepository = customPeriodRewardRepository;
    readonly IRepository<CustomPeriodChallenge> customPeriodChallengeRepository = customPeriodChallengeRepository;

    public async Task<GetCustomPeriodQueryModel> Handle(GetCustomPeriodQuery request, CancellationToken cancellationToken)
    {
        var rewards = await this.customPeriodRewardRepository.GetWithPropertiesAsync(x => new GetCustomPeriodRewardQueryModel
        {
            Id = x.Id,
            RequiredPoints = x.RequiredPoints,
            SelectedReward = x.RewardId
        }, cancellationToken);

        var challenges = await this.customPeriodChallengeRepository.GetWithPropertiesAsync(x => new GetCustomPeriodChallengeQueryModel
        {
            Id = x.Id,
            Attempts = x.Attempts,
            Points = x.RewardPoints,
            SelectedGame = x.GameId,
        }, cancellationToken);

        return new GetCustomPeriodQueryModel
        {
            Rewards = rewards,
            Challenges = challenges
        };
    }
}