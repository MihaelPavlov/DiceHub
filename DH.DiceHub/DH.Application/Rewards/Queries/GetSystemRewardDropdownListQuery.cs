using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetSystemRewardDropdownListQuery : IRequest<List<GetSystemRewardDropdownListQueryModel>>;

internal class GetSystemRewardDropdownListQueryHandler(
    IRepository<ChallengeReward> repository)
    : IRequestHandler<GetSystemRewardDropdownListQuery, List<GetSystemRewardDropdownListQueryModel>>
{
    readonly IRepository<ChallengeReward> repository = repository;

    public async Task<List<GetSystemRewardDropdownListQueryModel>> Handle(GetSystemRewardDropdownListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync<GetSystemRewardDropdownListQueryModel>(
            x => !x.IsDeleted, x => new(x.Id, x.Name_EN, x.Name_BG), cancellationToken);
    }
}