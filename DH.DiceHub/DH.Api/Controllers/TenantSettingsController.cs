using DH.Adapter.Authentication.Filters;
using DH.Application.Common.Commands;
using DH.Application.Common.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantSettingsController : ControllerBase
{
    readonly IMediator mediator;

    public TenantSettingsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("get-club-name")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> GetClubName(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetTenantSettingsQuery(), cancellationToken);
        return Ok(result.ClubName);
    }

    [HttpGet("club-info")]
    [ActionAuthorize(UserAction.TenantSettingsR)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetClubInfoModel))]
    public async Task<IActionResult> GetClubInfo(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetClubInfoQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ActionAuthorize(UserAction.TenantSettingsR)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantSettingDto))]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetTenantSettingsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [ActionAuthorize(UserAction.TenantSettingsCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantSettingDto))]
    public async Task<IActionResult> UpdateSettings([FromBody] TenantSettingDto command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new UpdateTenantSettingsCommand(command), cancellationToken);
        return Ok();
    }
}
