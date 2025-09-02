using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetActiveSpaceTableReservationListQuery : IRequest<List<GetActiveSpaceTableReservationListQueryModel>>;

internal class GetActiveSpaceTableReservationListQueryHandler : IRequestHandler<GetActiveSpaceTableReservationListQuery, List<GetActiveSpaceTableReservationListQueryModel>>
{
    readonly IRepository<SpaceTableReservation> repository;
    readonly IUserService userService;
    readonly IRepository<TenantUserSetting> tenantUserSettingRepository;
    readonly ILocalizationService localizer;
    public GetActiveSpaceTableReservationListQueryHandler(
        IRepository<SpaceTableReservation> repository, 
        IUserService userService,
        IRepository<TenantUserSetting> tenantUserSettingRepository,
        ILocalizationService localizer)
    {
        this.repository = repository;
        this.userService = userService;
        this.tenantUserSettingRepository = tenantUserSettingRepository;
        this.localizer = localizer;
    }

    public async Task<List<GetActiveSpaceTableReservationListQueryModel>> Handle(GetActiveSpaceTableReservationListQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.repository.GetWithPropertiesAsync(
            x => x.IsActive,
            x => new GetActiveSpaceTableReservationListQueryModel
            {
                Id = x.Id,
                UserId = x.UserId,
                CreatedDate = x.CreatedDate,
                ReservationDate = x.ReservationDate,
                IsActive = x.IsActive,
                Status = x.Status,
                NumberOfGuests = x.NumberOfGuests,
            }, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
            {
                reservation.Username = user.UserName;

                var tenantUserSettings = await this.tenantUserSettingRepository.GetByAsync(x => x.UserId == reservation.UserId, cancellationToken);

                reservation.PhoneNumber = tenantUserSettings?.PhoneNumber ?? this.localizer["NotProvided"];
                reservation.UserLanguage = tenantUserSettings?.Language ?? this.localizer["NotProvided"];
            }
        }

        return reservations.OrderByDescending(x => x.ReservationDate).ToList();
    }
}