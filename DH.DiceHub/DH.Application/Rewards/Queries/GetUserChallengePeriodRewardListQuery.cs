using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetUserChallengePeriodRewardListQuery(int PeriodPerformanceId) : IRequest<List<GetUserChallengePeriodRewardListQueryModel>>;

internal class GetUserChallengePeriodRewardListQueryHandler : IRequestHandler<GetUserChallengePeriodRewardListQuery, List<GetUserChallengePeriodRewardListQueryModel>>
{
    readonly IRepository<UserChallengePeriodReward> repository;
    readonly IUserContext userContext;

    public GetUserChallengePeriodRewardListQueryHandler(IRepository<UserChallengePeriodReward> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<List<GetUserChallengePeriodRewardListQueryModel>> Handle(GetUserChallengePeriodRewardListQuery request, CancellationToken cancellationToken)
    {
        var rewards= await this.repository.GetWithPropertiesAsync(
              x => x.UserChallengePeriodPerformanceId == request.PeriodPerformanceId &&
              this.userContext.UserId == x.UserChallengePeriodPerformance.UserId &&
              x.UserChallengePeriodPerformance.IsPeriodActive,
              x => new GetUserChallengePeriodRewardListQueryModel
              {
                  RewardImageId = x.ChallengeReward.Image.Id,
                  IsCompleted = x.IsCompleted,
                  RewardRequiredPoints = (int)x.ChallengeReward.RequiredPoints
              }, cancellationToken);

        return rewards.OrderBy(x => x.RewardRequiredPoints).ToList();
    }
}
