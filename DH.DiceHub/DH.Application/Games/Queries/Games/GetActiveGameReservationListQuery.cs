using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetActiveGameReservationListQuery : IRequest<List<GetActiveGameReservationListQueryModel>>;

internal class GetActiveGameReservationListQueryHandler(IGameService gameService, IUserService userService) : IRequestHandler<GetActiveGameReservationListQuery, List<GetActiveGameReservationListQueryModel>>
{
    readonly IGameService gameService = gameService;
    readonly IUserService userService = userService;

    public async Task<List<GetActiveGameReservationListQueryModel>> Handle(GetActiveGameReservationListQuery request, CancellationToken cancellationToken)
    {
        var activeReservations = await this.gameService.GetActiveGameReservation(cancellationToken);

        var userIds = activeReservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in activeReservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
                reservation.Username = user.UserName;
        }

        return activeReservations;
    }
}