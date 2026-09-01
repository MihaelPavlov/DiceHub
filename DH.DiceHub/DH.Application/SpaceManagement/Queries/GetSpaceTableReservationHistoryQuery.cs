using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Enums;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableReservationHistoryQuery(ReservationStatus? Status) : IRequest<List<GetSpaceTableReservationHistoryQueryModel>>;

internal class GetSpaceTableReservationHistoryQueryHandler(
    ISpaceTableService spaceTableService,
    IUserManagementService userManagementService) : IRequestHandler<GetSpaceTableReservationHistoryQuery, List<GetSpaceTableReservationHistoryQueryModel>>
{
    readonly IUserManagementService userManagementService = userManagementService;
    readonly ISpaceTableService spaceTableService = spaceTableService;

    public async Task<List<GetSpaceTableReservationHistoryQueryModel>> Handle(GetSpaceTableReservationHistoryQuery request, CancellationToken cancellationToken)
    {
        var reservations = await this.spaceTableService.GetSpaceTableReservationListByStatus(request.Status, cancellationToken);

        var userIds = reservations.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

        var users = await this.userManagementService.GetUserListByIds(userIds, cancellationToken);

        foreach (var reservation in reservations)
        {
            var user = users.FirstOrDefault(x => x.Id == reservation.UserId);

            if (user != null)
                reservation.Username = user.UserName;
        }

        return reservations;
    }
}
