using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries;

public record GetGameReservationHistoryQuery : IRequest<List<GetGameReservationHistoryQueryModel>>;

internal class GetGameReservationHistoryQueryHandler(IRepository<GameReservation> repository, IUserService userService) : IRequestHandler<GetGameReservationHistoryQuery, List<GetGameReservationHistoryQueryModel>>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IUserService userService = userService;

    public async Task<List<GetGameReservationHistoryQueryModel>> Handle(GetGameReservationHistoryQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.repository.GetWithPropertiesAsync(
            x => x.Status != ReservationStatus.None,
            x => new GetGameReservationHistoryQueryModel
            {
                Id = x.Id,
                UserId = x.UserId,
                CreatedDate = x.CreatedDate.ToLocalTime(),
                ReservationDate = x.ReservationDate.ToLocalTime(),
                NumberOfGuests = x.NumberOfGuests,
                IsActive = x.IsActive,
                IsReservationSuccessful = x.IsReservationSuccessful,
                Status = x.Status,
            }, cancellationToken);

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