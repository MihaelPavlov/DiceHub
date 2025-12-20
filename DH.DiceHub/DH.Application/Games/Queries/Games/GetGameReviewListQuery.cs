using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameReviewListQuery(int Id) : IRequest<List<GetGameReviewListQueryModel>>;

internal class GetGameReviewListQueryHandler : IRequestHandler<GetGameReviewListQuery, List<GetGameReviewListQueryModel>>
{
    readonly IRepository<GameReview> repository;
    readonly IUserManagementService userManagementService;

    public GetGameReviewListQueryHandler(
        IRepository<GameReview> repository,
        IUserManagementService userManagementService)
    {
        this.repository = repository;
        this.userManagementService = userManagementService;
    }

    public async Task<List<GetGameReviewListQueryModel>> Handle(GetGameReviewListQuery request, CancellationToken cancellationToken)
    {
        var gameReviews = await this.repository.GetWithPropertiesAsync<GetGameReviewListQueryModel>(
            x => x.GameId == request.Id,
            x => new()
            {
                Id = x.Id,
                GameId = x.GameId,
                Review = x.Review,
                UserId = x.UserId,
                UpdatedDate = x.UpdatedDate
            }, cancellationToken);

        var userIds = gameReviews.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userManagementService.GetUserListByIds(userIds, cancellationToken);

        foreach (var gameReview in gameReviews)
        {
            var user = users.FirstOrDefault(x => x.Id == gameReview.UserId);
            if (user != null)
            {
                gameReview.UserFullName = user.UserName;
                gameReview.UserImageUrl = user.ImageUrl;

            }
        }

        return gameReviews.OrderByDescending(x => x.UpdatedDate).ToList();
    }
}
