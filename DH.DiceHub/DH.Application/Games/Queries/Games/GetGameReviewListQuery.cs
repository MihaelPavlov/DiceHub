using DH.Application.Cqrs;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;

namespace DH.Application.Games.Queries.Games;

public record GetGameReviewListQuery(int Id) : ICommand<List<GetGameReviewListQueryModel>>;

internal class GetGameReviewListQueryHandler : AbstractCommandHandler<GetGameReviewListQuery, List<GetGameReviewListQueryModel>>
{
    readonly IRepository<GameReview> repository;
    readonly IUserService userService;

    public GetGameReviewListQueryHandler(IRepository<GameReview> repository, IUserService userService)
    {
        this.repository = repository;
        this.userService = userService;
    }

    protected override async Task<List<GetGameReviewListQueryModel>> HandleAsync(GetGameReviewListQuery request, CancellationToken cancellationToken)
    {
        var gameReviews = await this.repository.GetWithPropertiesAsync<GetGameReviewListQueryModel>(
            x => x.GameId == request.Id,
            x => new()
            {
                GameId = x.GameId,
                Review = x.Comment,
                UserId = x.UserId
            }, cancellationToken);

        var userIds = gameReviews.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var gameReview in gameReviews)
        {
            var user = users.FirstOrDefault(x => x.Id == gameReview.UserId);
            if (user != null)
            {
                gameReview.UserFullName = user.UserName;
                gameReview.UserImageUrl = user.ImageUrl;

            }
        }

        return gameReviews;
    }
}
