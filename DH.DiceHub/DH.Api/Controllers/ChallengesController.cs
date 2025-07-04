using DH.Adapter.Authentication.Filters;
using DH.Application.Challenges.Commands;
using DH.Application.Challenges.Qureies;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Models.ChallengeModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
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

    [HttpGet("get-user-challenges")]
    [ActionAuthorize(UserAction.ChallengesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserChallengeListQueryModel>))]
    public async Task<IActionResult> GetUserChallengeList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserChallengeListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-user-challenge-period-performance")]
    [ActionAuthorize(UserAction.ChallengesRead)]
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
    [HttpGet("get-custom-period")]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomPeriodQueryModel))]
    public async Task<IActionResult> GetCustomPeriod(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetCustomPeriodQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-user-custom-period")]
    [ActionAuthorize(UserAction.ChallengesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCustomPeriodQueryModel))]
    public async Task<IActionResult> GetUserCustomPeriod(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserCustomPeriodQuery(), cancellationToken);
        return Ok(result);
    }
    [HttpPost("save-custom-period")]
    [ActionAuthorize(UserAction.ChallengesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveCustomPeriod([FromBody] SaveCustomPeriodCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
}
