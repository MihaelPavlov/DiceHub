using DH.Adapter.Authentication.Filters;
using DH.Application.Common.Commands;
using DH.Application.Common.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TenantUserSettingsController : ControllerBase
{
    readonly IMediator mediator;

    public TenantUserSettingsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [ActionAuthorize(UserAction.TenantUserSettingsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSettingsDto))]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserSettingsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [ActionAuthorize(UserAction.TenantUserSettingsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSettingsDto))]
    public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsDto command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new UpdateUserSettingsCommand(command), cancellationToken);
        return Ok();
    }

    [HttpGet("assistive-touch-settings")]
    [ActionAuthorize(UserAction.TenantUserSettingsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssistiveTouchSettings))]
    public async Task<IActionResult> GetAssistiveTouchSettings(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetAssistiveTouchSettingsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("assistive-touch-settings")]
    [ActionAuthorize(UserAction.TenantUserSettingsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAssistiveTouchSettings([FromBody] UpdateAssistiveTouchSettingsCommand settings, CancellationToken cancellationToken)
    {
        await this.mediator.Send(settings, cancellationToken);
        return Ok();
    }
}
