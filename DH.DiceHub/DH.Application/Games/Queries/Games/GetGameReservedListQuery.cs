using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameReservedListQuery : IRequest<List<GetGameReservationListQueryModel>>;

internal class GetGameReservedListQueryHandler : IRequestHandler<GetGameReservedListQuery, List<GetGameReservationListQueryModel>>
{
    readonly IRepository<GameReservation> repository;
    readonly IUserService userService;
    readonly IRepository<Game> gameRepository;
    public GetGameReservedListQueryHandler(IRepository<GameReservation> repository, IUserService userService, IRepository<Game> gameRepository)
    {
        this.repository = repository;
        this.userService = userService;
        this.gameRepository = gameRepository;
    }

    public async Task<List<GetGameReservationListQueryModel>> Handle(GetGameReservedListQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.repository.GetWithPropertiesAsync<GetGameReservationListQueryModel>(x => new GetGameReservationListQueryModel
        {
            UserId = x.UserId,
            ReservationDate = x.ReservationDate,
            ReservedDurationMinutes = x.ReservedDurationMinutes,
            IsExpired = x.IsExpired,
            IsPaymentSuccessful = x.IsPaymentSuccessful,
            GameId = x.GameId
        }, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        var gameIds = reservations.DistinctBy(x => x.GameId).Select(x => x.GameId).ToArray();

        var games = await this.gameRepository.GetWithPropertiesAsync<GameRecord>(
            x => gameIds.Contains(x.Id),
            x => new GameRecord(x.Id, x.Image.Id, x.Name),
            cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);
            if (user != null)
            {
                reservation.UserName = user.UserName;
            }

            var game = games.FirstOrDefault(x => x.Id == reservation.GameId);
            if (game != null)
            {
                reservation.GameImageId = game.ImageId;
                reservation.GameName = game.Name;
            }
        }

        return reservations.OrderByDescending(x => x.ReservationDate).ToList();
    }
    private record GameRecord(int Id, int ImageId, string Name);
}

