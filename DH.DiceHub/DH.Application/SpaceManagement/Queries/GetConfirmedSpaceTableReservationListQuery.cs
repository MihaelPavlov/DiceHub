using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetConfirmedSpaceTableReservationListQuery : IRequest<List<GetConfirmedSpaceTableReservationListQueryModel>>;

internal class GetConfirmedSpaceTableReservationListQueryHandler(IRepository<SpaceTableReservation> repository, IUserService userService) : IRequestHandler<GetConfirmedSpaceTableReservationListQuery, List<GetConfirmedSpaceTableReservationListQueryModel>>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IUserService userService = userService;

    public async Task<List<GetConfirmedSpaceTableReservationListQueryModel>> Handle(GetConfirmedSpaceTableReservationListQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.repository.GetWithPropertiesAsync<GetConfirmedSpaceTableReservationListQueryModel>(
            x => x.Status != ReservationStatus.None,
            x => new GetConfirmedSpaceTableReservationListQueryModel
            {
                Id = x.Id,
                UserId = x.UserId,
                CreatedDate = x.CreatedDate,
                ReservationDate = x.ReservationDate,
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
