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

    [HttpPost("list")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSpaceTableListQueryModel>))]
    public async Task<IActionResult> GetSpaceTableList([FromBody] GetSpaceTableListQuery query, CancellationToken cancellationToken)
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

    [HttpPut("{id}/join")]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSpaceTableListQueryModel>))]
    public async Task<IActionResult> JoinSpaceTable([FromQuery] int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new JoinSpaceTableCommand(id), cancellationToken);
        return Ok();
    }
}
