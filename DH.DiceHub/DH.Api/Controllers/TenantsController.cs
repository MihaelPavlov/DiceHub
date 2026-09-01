using DH.Application.Common.Queries;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
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
    readonly IUserContext userContext;

    public TenantsController(IMediator mediator, IUserContext userContext)
    {
        this.mediator = mediator;
        this.userContext = userContext;
    }

    [HttpGet("list")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetTenantListQueryModel>))]
    public async Task<IActionResult> GetClubs(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetTenantListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{tenantId}")]
    [Authorize(Roles = "SuperAdmin,Owner")]
    public async Task<IActionResult> GetTenant(string tenantId, CancellationToken cancellationToken)
    {
        // Owners may only read their own tenant - this query returns full
        // tenant-application PII (contact email/phone/address), so a non-
        // SuperAdmin caller must not be able to fetch an arbitrary tenant by id.
        if (this.userContext.RoleKey != (int)Role.SuperAdmin && tenantId != this.userContext.TenantId)
        {
            return Forbid();
        }

        var result = await this.mediator.Send(new GetTenantByIdQuery(tenantId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("tenant-setup/exists")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> TenantSetupExists([FromQuery] string? token, CancellationToken cancellationToken)
    {
        var exists = await this.mediator.Send(new ValidateTenantSetupTokenQuery(token), cancellationToken);
        if (!exists) return NotFound();
        return Ok(true);
    }

    [HttpGet("{tenantId}/exists")]
    public async Task<ActionResult<bool>> TenantExists(string tenantId, CancellationToken cancellationToken)
    {
        var tenant = await this.mediator.Send(new GetTenantByIdQuery(tenantId), cancellationToken);
        if (tenant == null) return NotFound();
        return Ok(true);
    }
}
