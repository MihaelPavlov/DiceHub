﻿using DH.Adapter.Authentication.Filters;
using DH.Application.SpaceManagement.Commands;
using DH.Application.SpaceManagement.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.SpaceManagementModels.Commands;
using DH.Domain.Models.SpaceManagementModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SpaceManagementController : ControllerBase
{
    readonly IMediator mediator;

    public SpaceManagementController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("get-table-participants")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSpaceTableParticipantListQueryModel>))]
    public async Task<IActionResult> GetTableParticipant([FromBody] GetSpaceTableParticipantListQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-user-active-table")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserActiveTableQueryModel))]
    public async Task<IActionResult> GetUserActiveTable(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserActiveTableQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-space-activity-stats")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSpaceActivityStatsQuery))]
    public async Task<IActionResult> GetSpaceActivityStats(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetSpaceActivityStatsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-space-available-tables")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSpaceAvailableTableListQueryModel>))]
    public async Task<IActionResult> GetSpaceAvailableTableList([FromBody] GetSpaceAvailableTableListQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateSpaceTable([FromBody] CreateSpaceTableDto command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateSpaceTableCommand(command), cancellationToken);
        return Ok(result);
    }

    [HttpPost("leave-table")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LeaveSpaceTable([FromBody] LeaveSpaceTableCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("remove-user-from-table")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveUserFromSpaceTable([FromBody] RemoveUserFromSpaceTableCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("close-table")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CloseSpaceTable([FromBody] CloseSpaceTableCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut("join")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> JoinSpaceTable([FromBody] JoinSpaceTableCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
}
