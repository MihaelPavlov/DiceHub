using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetAllSystemRewardListQuery : IRequest<List<GetSystemRewardListQueryModel>>;

internal class GetAllSystemRewardListQueryHandler(IRepository<ChallengeReward> challengeRewardRepository) : IRequestHandler<GetAllSystemRewardListQuery, List<GetSystemRewardListQueryModel>>
{
    readonly IRepository<ChallengeReward> challengeRewardRepository = challengeRewardRepository;

    public async Task<List<GetSystemRewardListQueryModel>> Handle(GetAllSystemRewardListQuery request, CancellationToken cancellationToken)
    {
        return await this.challengeRewardRepository.GetWithPropertiesAsync(
           x => new GetSystemRewardListQueryModel
           {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description,
               ImageId = x.Image.Id
           }, cancellationToken);
    }
}