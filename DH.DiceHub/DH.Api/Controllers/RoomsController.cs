﻿using DH.Adapter.Authentication.Filters;
using DH.Application.Rooms.Commands;
using DH.Application.Rooms.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.RoomModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomsController : ControllerBase
{
    readonly IMediator mediator;

    public RoomsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("list")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRoomListQueryModel>))]
    public async Task<IActionResult> CreateRoom(GetRoomListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}