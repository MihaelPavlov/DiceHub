using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Stats.Queries;

public record GetUserStatsQuery : IRequest<GetUserStatsQueryModel>;

internal class GetUserStatsQueryHandler(
    IUserContext userContext,
    IRepository<GameReservation> gameReservationRepository,
    IRepository<SpaceTableReservation> spaceTableReservationRepository,
    IRepository<EventParticipant> eventParticipantRepository) : IRequestHandler<GetUserStatsQuery, GetUserStatsQueryModel>
{
    readonly IUserContext userContext = userContext;
    readonly IRepository<GameReservation> gameReservationRepository = gameReservationRepository;
    readonly IRepository<SpaceTableReservation> spaceTableReservationRepository = spaceTableReservationRepository;
    readonly IRepository<EventParticipant> eventParticipantRepository = eventParticipantRepository;

    public async Task<GetUserStatsQueryModel> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        var games = await this.gameReservationRepository.GetWithPropertiesAsync(
            x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        var uniqueGamesPlayed = games.Select(x => x.GameId).Distinct().Count();

        var spaceTableReservations = await this.spaceTableReservationRepository.GetWithPropertiesAsync(
            x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        var reservationsCount = games.Count + spaceTableReservations.Count;

        var events = await this.eventParticipantRepository.GetWithPropertiesAsync(
            x => x.UserId == this.userContext.UserId, x => x, cancellationToken);

        return new GetUserStatsQueryModel(uniqueGamesPlayed, reservationsCount, events.Count);
    }
}

public record GetUserStatsQueryModel(int UniquePlayedGames, int Reservations, int Events);