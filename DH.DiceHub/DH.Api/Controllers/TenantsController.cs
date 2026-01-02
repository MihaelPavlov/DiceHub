using DH.Application.Common.Queries;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace DH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    readonly IMediator mediator;

    public TenantsController(IMediator mediator)
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

    [HttpGet("{tenantId}/exists")]
    public async Task<ActionResult<bool>> TenantExists(string tenantId, CancellationToken cancellationToken)
    {
        var tenant = await this.mediator.Send(new GetTenantByIdQuery(tenantId), cancellationToken);
        if (tenant == null) return NotFound();
        return Ok(true);
    }
}
