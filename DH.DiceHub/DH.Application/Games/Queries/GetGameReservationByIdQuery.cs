using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Games.Queries;

public record GetGameReservationByIdQuery(int Id) : IRequest<GetGameReservationByIdQueryModel>;

internal class GetGameReservationByIdQueryHandler(IRepository<GameReservation> repository, IUserService userService) : IRequestHandler<GetGameReservationByIdQuery, GetGameReservationByIdQueryModel>
{
    readonly IRepository<GameReservation> repository = repository;
    readonly IUserService userService = userService;

    public async Task<GetGameReservationByIdQueryModel> Handle(GetGameReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservationDb = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameReservation), request.Id);

        var users = await this.userService.GetUserListByIds([reservationDb.UserId], cancellationToken);
        reservationDb.ReservationDate = reservationDb.ReservationDate.ToLocalTime();
        reservationDb.CreatedDate = reservationDb.CreatedDate.ToLocalTime();

        var reservation = reservationDb.Adapt<GetGameReservationByIdQueryModel>();
        var user = users.FirstOrDefault(x => x.Id == reservationDb.UserId);

        if (user != null)
            reservation.Username = user.UserName;

        return reservation;
    }
}
