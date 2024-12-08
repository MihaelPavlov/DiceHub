using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetReservationByIdQuery(int Id) : IRequest<GetReservationByIdQueryModel>;

internal class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, GetReservationByIdQueryModel>
{
    readonly IRepository<SpaceTableReservation> repository;
    readonly IUserService userService;

    public GetReservationByIdQueryHandler(IRepository<SpaceTableReservation> repository, IUserService userService)
    {
        this.repository = repository;
        this.userService = userService;
    }

    public async Task<GetReservationByIdQueryModel> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservationDb = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        var users = await this.userService.GetUserListByIds([reservationDb.UserId], cancellationToken);

        var reservation = reservationDb.Adapt<GetReservationByIdQueryModel>();
        var user = users.FirstOrDefault(x => x.Id == reservationDb.UserId);

        if (user != null)
            reservation.Username = user.UserName;

        return reservation;
    }
}
