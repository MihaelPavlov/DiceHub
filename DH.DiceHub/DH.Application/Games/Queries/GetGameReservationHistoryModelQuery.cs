using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries;

public record GetGameReservationHistoryQuery(ReservationStatus? Status) : IRequest<List<GetGameReservationHistoryQueryModel>>;

internal class GetGameReservationHistoryQueryHandler(
    IUserManagementService userManagementService, IGameService gameService) : IRequestHandler<GetGameReservationHistoryQuery, List<GetGameReservationHistoryQueryModel>>
{
    readonly IUserManagementService userManagementService = userManagementService;
    readonly IGameService gameService = gameService;

    public async Task<List<GetGameReservationHistoryQueryModel>> Handle(GetGameReservationHistoryQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.gameService.GetGameReservationListByStatus(request.Status, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userManagementService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
                reservation.Username = user.UserName;
        }

        return reservations;
    }
}