using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetActiveBookedSpaceTableQuery : IRequest<GetActiveBookedSpaceTableQueryModel?>;

internal class GetActiveBookedSpaceTableQueryHandler(
    IUserContext userContext, 
    IUserManagementService userManagementService, 
    IRepository<SpaceTableReservation> repository) : IRequestHandler<GetActiveBookedSpaceTableQuery, GetActiveBookedSpaceTableQueryModel?>
{
    readonly IUserContext userContext = userContext;
    readonly IUserManagementService userManagementService = userManagementService;
    readonly IRepository<SpaceTableReservation> repository = repository;

    public async Task<GetActiveBookedSpaceTableQueryModel?> Handle(GetActiveBookedSpaceTableQuery request, CancellationToken cancellationToken)
    {
        var userReservation = await this.repository.GetByAsync(x => x.IsActive && x.UserId == this.userContext.UserId, cancellationToken);

        if (userReservation == null)
            return null;

        var users = await this.userManagementService.GetUserListByIds([userReservation.UserId], cancellationToken);

        return new GetActiveBookedSpaceTableQueryModel
        {
            Id = userReservation.Id,
            CreatedDate = userReservation.CreatedDate,
            ReservationDate = userReservation.ReservationDate,
            IsActive = userReservation.IsActive,
            NumberOfGuests = userReservation.NumberOfGuests,
            Username = users.First().UserName,
            Status = userReservation.Status,
            PublicNote = userReservation.PublicNote,
        };
    }
}
