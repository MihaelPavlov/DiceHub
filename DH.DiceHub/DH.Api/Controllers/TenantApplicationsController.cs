using DH.Adapter.Authentication.Filters;
using DH.Application.Common.Commands;
using DH.Application.Common.Queries;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantApplicationsController(IMediator mediator) : ControllerBase
{
    readonly IMediator mediator = mediator;

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    public async Task<IActionResult> Submit([FromBody] TenantApplicationRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateTenantApplicationCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [AllowAnonymous]
    [HttpPost("send-email-verification-code")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> SendEmailVerificationCode([FromBody] TenantApplicationSendEmailCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SendTenantApplicationEmailVerificationCodeCommand(request), cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("verify-email-code")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> VerifyEmailCode([FromBody] TenantApplicationVerifyEmailCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new VerifyTenantApplicationEmailVerificationCodeCommand(request), cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("setup/seed-games")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSeedGameCatalogDropdownListQueryModel>))]
    public async Task<IActionResult> GetSetupSeedGames(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSeedGameCatalogDropdownListQuery(), cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("setup/complete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompleteTenantSetupResult))]
    public async Task<IActionResult> CompleteSetup([FromBody] CompleteTenantSetupRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CompleteTenantSetupCommand(request), cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    [ActionAuthorize(UserAction.TenantApplicationsReview)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TenantApplicationDto>))]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTenantApplicationsQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ActionAuthorize(UserAction.TenantApplicationsReview)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantApplicationDto))]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTenantApplicationByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id:int}/verify")]
    [ActionAuthorize(UserAction.TenantApplicationsReview)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Verify(int id, [FromBody] TenantApplicationReviewRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReviewTenantApplicationCommand(id, TenantApplicationStatus.Verified, request.Note), cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpPost("{id:int}/reject")]
    [ActionAuthorize(UserAction.TenantApplicationsReview)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(int id, [FromBody] TenantApplicationReviewRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReviewTenantApplicationCommand(id, TenantApplicationStatus.Rejected, request.Note), cancellationToken);
        return Ok();
    }
}
