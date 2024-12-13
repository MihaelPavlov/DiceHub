using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetActiveReservedGameQuery : IRequest<GetActiveReservedGameQueryModel?>;

internal class GetActiveReservedGameQueryHandler : IRequestHandler<GetActiveReservedGameQuery, GetActiveReservedGameQueryModel?>
{
    readonly IRepository<GameReservation> repository;
    readonly IUserContext userContext;
    public GetActiveReservedGameQueryHandler(IRepository<GameReservation> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<GetActiveReservedGameQueryModel?> Handle(GetActiveReservedGameQuery request, CancellationToken cancellationToken)
    {
        var activeUserReservation = await this.repository.GetByAsync(
            x => x.UserId == this.userContext.UserId && x.IsActive && x.Status == ReservationStatus.Accepted, cancellationToken);

        if (activeUserReservation == null)
            return null;

        return activeUserReservation.Adapt<GetActiveReservedGameQueryModel>();
    }
}
