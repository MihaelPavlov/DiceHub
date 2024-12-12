using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameReservationStatusQuery(int Id) : IRequest<GetGameReservationStatusQueryModel?>;

internal class GetGameReservationStatusQueryHandler : IRequestHandler<GetGameReservationStatusQuery, GetGameReservationStatusQueryModel?>
{
    readonly IRepository<GameReservation> repository;
    readonly IUserContext userContext;
    public GetGameReservationStatusQueryHandler(IRepository<GameReservation> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<GetGameReservationStatusQueryModel?> Handle(GetGameReservationStatusQuery request, CancellationToken cancellationToken)
    {
        var userReservationList = await this.repository.GetWithPropertiesAsync(
            x => x.UserId == this.userContext.UserId && x.GameId == request.Id,
            x => new GetGameReservationStatusQueryModel
            {
                ReservationId = x.Id,
                GameId = x.GameId,
                ReservationDate = x.ReservationDate,
                ReservedDurationMinutes = x.ReservedDurationMinutes,
                IsActive = x.IsActive,
                Status = x.Status,
            }, cancellationToken);

        return userReservationList
            .OrderByDescending(x => x.ReservationDate)
            .FirstOrDefault(x => x.IsActive);
    }
}
