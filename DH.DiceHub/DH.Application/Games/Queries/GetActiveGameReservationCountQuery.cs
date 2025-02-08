using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries;

public record GetActiveGameReservationCountQuery : IRequest<int>;

internal class GetActiveGameReservationCountQueryHandler(IGameService gameService) : IRequestHandler<GetActiveGameReservationCountQuery, int>
{
    readonly IGameService gameService = gameService;

    public async Task<int> Handle(GetActiveGameReservationCountQuery request, CancellationToken cancellationToken)
    {
        return await this.gameService.GetActiveGameReservationsCount(cancellationToken);
    }
}