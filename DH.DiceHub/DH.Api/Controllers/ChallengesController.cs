﻿using DH.Adapter.Authentication.Filters;
using DH.Application.Challenges.Commands;
using DH.Application.Challenges.Qureies;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Models.ChallengeModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ChallengesController : ControllerBase
{
    readonly IMediator mediator;

    public ChallengesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.ChallengesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetChallengeByIdQueryModel))]
    public async Task<IActionResult> GetChallengeById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetChallengeByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    //TODO: Use it in FE
    [HttpGet("get-user-challenges")]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserChallengeListQueryModel>))]
    public async Task<IActionResult> GetUserChallengeList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserChallengeListQuery(), cancellationToken);
        return Ok(result);
    }

    //TODO: Use it in FE
    [HttpGet("get-user-challenge-period-performance")]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserChallengePeriodPerformanceQueryModel))]
    public async Task<IActionResult> GetUserChallengePeriodPerformnace(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserChallengePeriodPerformanceQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("list")]
    [ActionAuthorize(UserAction.ChallengesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetChallengeListWithFilterQueryModel>))]
    public async Task<IActionResult> GetChallengeList(GetChallengeListWithFilterQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateChallenge([FromBody] CreateChallengeDto command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateChallengeCommand(command), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateChallenge([FromBody] UpdateChallengeDto command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new UpdateChallengeCommand(command), cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteChallenge(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteChallengeCommand(id), cancellationToken);
        return Ok();
    }
}
