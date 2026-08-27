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

    [HttpGet("logo")]
    [ActionAuthorize(UserAction.TenantSettingsR)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> GetLogo(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetTenantLogoQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("logo")]
    [ActionAuthorize(UserAction.TenantSettingsCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> UpdateLogo([FromForm] IFormFile logoFile, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await logoFile.CopyToAsync(stream, cancellationToken);

        var logoUrl = await this.mediator.Send(new UpdateTenantLogoCommand(logoFile.FileName, stream), cancellationToken);
        return Ok(logoUrl);
    }
}
