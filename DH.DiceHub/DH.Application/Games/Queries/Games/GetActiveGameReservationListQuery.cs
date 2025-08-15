using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetActiveGameReservationListQuery : IRequest<List<GetActiveGameReservationListQueryModel>>;

internal class GetActiveGameReservationListQueryHandler(
    IGameService gameService, IUserService userService,
    IRepository<TenantUserSetting> repository, ILocalizationService localizer) : IRequestHandler<GetActiveGameReservationListQuery, List<GetActiveGameReservationListQueryModel>>
{
    readonly IGameService gameService = gameService;
    readonly IUserService userService = userService;
    readonly IRepository<TenantUserSetting> repository = repository;
    readonly ILocalizationService localizer = localizer;

    public async Task<List<GetActiveGameReservationListQueryModel>> Handle(GetActiveGameReservationListQuery request, CancellationToken cancellationToken)
    {
        var activeReservations = await this.gameService.GetActiveGameReservation(cancellationToken);

        var userIds = activeReservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in activeReservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
            {
                reservation.Username = user.UserName;

                var tenantUserSettings = await this.repository.GetByAsync(x => x.UserId == reservation.UserId, cancellationToken);

                reservation.PhoneNumber = tenantUserSettings?.PhoneNumber ?? this.localizer["NotProvided"];
            }

        }

        return activeReservations;
    }
}