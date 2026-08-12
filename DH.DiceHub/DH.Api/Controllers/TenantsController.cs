using DH.Application.Common.Queries;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    readonly IMediator mediator;
    readonly TenantProvisioningService tenantProvisioningService;

    public TenantsController(IMediator mediator, TenantProvisioningService tenantProvisioningService)
    {
        this.mediator = mediator;
        this.tenantProvisioningService = tenantProvisioningService;
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
    [AllowAnonymous]
    public async Task<ActionResult<bool>> TenantExists(string tenantId, CancellationToken cancellationToken)
    {
        var tenant = await this.mediator.Send(new GetTenantByIdQuery(tenantId), cancellationToken);
        if (tenant == null) return NotFound();
        return Ok(true);
    }

    [Authorize]
    [HttpPost("provision")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateTenantResult))]
    public async Task<IActionResult> ProvisionTenant([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await this.tenantProvisioningService.ProvisionAsync(request, cancellationToken);
        return Ok(result);
    }
}
