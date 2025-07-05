using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetUserRewardListQuery : IRequest<List<GetUserRewardListQueryModel>>;

internal class GetUserRewardListQueryHandler : IRequestHandler<GetUserRewardListQuery, List<GetUserRewardListQueryModel>>
{
    readonly IRepository<UserChallengeReward> repository;
    readonly IUserContext userContext;

    public GetUserRewardListQueryHandler(IRepository<UserChallengeReward> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<List<GetUserRewardListQueryModel>> Handle(GetUserRewardListQuery request, CancellationToken cancellationToken)
    {
        var rewards = await this.repository.GetWithPropertiesAsync(
            x => x.UserId == this.userContext.UserId,
            x => new GetUserRewardListQueryModel
            {
                Id = x.Id,
                RewardImageId = x.Reward.Image.Id,
                RewardName = x.Reward.Name,
                RewardDescription = x.Reward.Description,
                AvailableMoreForDays = (x.ExpiresDate - DateTime.UtcNow).Days,
                Status = x.IsClaimed ? UserRewardStatus.Used :
                    x.IsExpired ? UserRewardStatus.Expired : UserRewardStatus.NotExpired
            }, cancellationToken);

        return rewards
            .OrderBy(r =>
            {
                // Primary sort: Group by status priority: NotExpired (0), Used (1), Expired (2)
                return r.Status switch
                {
                    UserRewardStatus.NotExpired => 0,
                    UserRewardStatus.Used => 1,
                    UserRewardStatus.Expired => 2,
                    _ => 3
                };
            })
            .ThenBy(r =>
            {
                // Secondary sort: For NotExpired, sort by days left (ascending)
                return r.Status == UserRewardStatus.NotExpired
                    ? r.AvailableMoreForDays
                    : int.MaxValue;
            })
            .ToList();
    }
}
