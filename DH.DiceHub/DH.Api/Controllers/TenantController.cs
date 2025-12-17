using DH.Application.Common.Queries;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    readonly IMediator mediator;

    public TenantController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("list")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetTenantListQueryModel>))]
    public async Task<IActionResult> GetClubs(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetTenantListQuery(), cancellationToken);
        return Ok(result);
    }
}
