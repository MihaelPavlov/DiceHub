using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries;

public record GetGameReservationHistoryQuery(ReservationStatus? Status) : IRequest<List<GetGameReservationHistoryQueryModel>>;

internal class GetGameReservationHistoryQueryHandler(IUserService userService, IGameService gameService) : IRequestHandler<GetGameReservationHistoryQuery, List<GetGameReservationHistoryQueryModel>>
{
    readonly IUserService userService = userService;
    readonly IGameService gameService = gameService;

    public async Task<List<GetGameReservationHistoryQueryModel>> Handle(GetGameReservationHistoryQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.gameService.GetGameReservationByStatus(request.Status, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
                reservation.Username = user.UserName;
        }

        return reservations
            .OrderBy(x => x.Status == ReservationStatus.Accepted || x.Status == ReservationStatus.Declined)
            .ThenByDescending(x => x.ReservationDate)
            .ToList();
    }
}