using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetSystemRewardListQuery(string? SearchExpression) : IRequest<List<GetSystemRewardListQueryModel>>;

internal class GetSystemRewardListQueryHandler : IRequestHandler<GetSystemRewardListQuery, List<GetSystemRewardListQueryModel>>
{
    readonly IRepository<ChallengeReward> challengeRewardRepository;

    public GetSystemRewardListQueryHandler(IRepository<ChallengeReward> challengeRewardRepository)
    {
        this.challengeRewardRepository = challengeRewardRepository;
    }

    public async Task<List<GetSystemRewardListQueryModel>> Handle(GetSystemRewardListQuery request, CancellationToken cancellationToken)
    {
        var searchExpression = request.SearchExpression ?? string.Empty;
        return await this.challengeRewardRepository.GetWithPropertiesAsync(
            x => !x.IsDeleted && x.Name.ToLower().Contains(searchExpression.ToLower()),
            x => new GetSystemRewardListQueryModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ImageId = x.Image.Id
            }, cancellationToken);
    }
}