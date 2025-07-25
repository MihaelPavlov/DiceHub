﻿using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableReservationByIdQuery(int Id) : IRequest<GetSpaceTableReservationByIdQueryModel>;

internal class GetSpaceTableReservationByIdQueryHandler : IRequestHandler<GetSpaceTableReservationByIdQuery, GetSpaceTableReservationByIdQueryModel>
{
    readonly IRepository<SpaceTableReservation> repository;
    readonly IUserService userService;

    public GetSpaceTableReservationByIdQueryHandler(IRepository<SpaceTableReservation> repository, IUserService userService)
    {
        this.repository = repository;
        this.userService = userService;
    }

    public async Task<GetSpaceTableReservationByIdQueryModel> Handle(GetSpaceTableReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservationDb = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.Id);

        var users = await this.userService.GetUserListByIds([reservationDb.UserId], cancellationToken);
        reservationDb.ReservationDate = reservationDb.ReservationDate;
        reservationDb.CreatedDate = reservationDb.CreatedDate;

        var reservation = reservationDb.Adapt<GetSpaceTableReservationByIdQueryModel>();
        var user = users.FirstOrDefault(x => x.Id == reservationDb.UserId);

        if (user != null)
            reservation.Username = user.UserName;

        return reservation;
    }
}
