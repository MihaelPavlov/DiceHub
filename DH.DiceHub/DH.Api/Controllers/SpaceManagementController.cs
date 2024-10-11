using DH.Adapter.Authentication.Filters;
using DH.Application.SpaceManagement.Commands;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.SpaceManagementModels.Commands;
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

    [HttpPost]
    [ActionAuthorize(UserAction.SpaceManagementCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateSpaceTable([FromBody] CreateSpaceTableDto command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateSpaceTableCommand(command), cancellationToken);
        return Ok(result);
    }
}
