using DH.Adapter.Authentication.Filters;
using DH.Application.Rooms.Commands;
using DH.Application.Rooms.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.RoomModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
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
    public async Task<IActionResult> GetRoomList(GetRoomListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/messages")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRoomMessageListQueryModel>))]
    public async Task<IActionResult> GetMessageList(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetRoomMessageListQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/info-messages")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRoomInfoMessageListQueryModel>))]
    public async Task<IActionResult> GetInfoMessageList(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetRoomInfoMessageListQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("members")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRoomMemberListQueryModel>))]
    public async Task<IActionResult> GetMemberList([FromBody] GetRoomMemberListQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("remove-member")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> RemoveMember([FromBody] RemoveRoomMemberCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRoomByIdQueryModel))]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetRoomByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/check-user-participation")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CheckRoomParticipation(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserRoomParticipationStatusQuery(id), cancellationToken);
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

    [HttpPut]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("join")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> JoinRoom([FromBody] JoinRoomCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("leave")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> LeaveRoom([FromBody] LeaveRoomCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ActionAuthorize(UserAction.RoomsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteRoomCommand(id), cancellationToken);
        return Ok();
    }
}
