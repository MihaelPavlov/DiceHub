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
            x => !x.IsDeleted && (x.Name_EN.ToLower().Contains(searchExpression.ToLower()) || x.Name_BG.ToLower().Contains(searchExpression.ToLower())),
            x => new GetSystemRewardListQueryModel
            {
                Id = x.Id,
                Name_EN = x.Name_EN,
                Name_BG = x.Name_BG,
                CashEquivalent = x.CashEquivalent,
                Description_EN = x.Description_EN,
                Description_BG = x.Description_BG,
                ImageUrl = x.ImageUrl
            }, cancellationToken);
    }
}