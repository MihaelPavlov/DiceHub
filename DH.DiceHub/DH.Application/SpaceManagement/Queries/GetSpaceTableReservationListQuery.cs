using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableReservationListQuery : IRequest<List<GetSpaceTableReservationListQueryModel>>;

internal class GetSpaceTableReservationListQueryHandler : IRequestHandler<GetSpaceTableReservationListQuery, List<GetSpaceTableReservationListQueryModel>>
{
    readonly IRepository<SpaceTableReservation> repository;
    readonly IUserService userService;

    public GetSpaceTableReservationListQueryHandler(IRepository<SpaceTableReservation> repository, IUserService userService)
    {
        this.repository = repository;
        this.userService = userService;
    }

    public async Task<List<GetSpaceTableReservationListQueryModel>> Handle(GetSpaceTableReservationListQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.repository.GetWithPropertiesAsync<GetSpaceTableReservationListQueryModel>(x => new GetSpaceTableReservationListQueryModel
        {
            Id = x.Id,
            UserId = x.UserId,
            CreatedDate = x.CreatedDate,
            ReservationDate = x.ReservationDate,
            IsActive = x.IsActive,
            IsConfirmed = x.IsConfirmed,
            IsReservationSuccessful = x.IsReservationSuccessful,
            NumberOfGuests = x.NumberOfGuests
        }, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
                reservation.Username = user.UserName;
        }

        return reservations.OrderByDescending(x => x.ReservationDate).ToList();
    }
}