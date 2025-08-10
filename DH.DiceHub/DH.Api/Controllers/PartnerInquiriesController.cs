using DH.Application.Common.Commands;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class PartnerInquiriesController : ControllerBase
{
    readonly IMediator mediator;

    public PartnerInquiriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitInquiry([FromBody] PartnerInquiryDto request, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new CreatePartnerInquiriesCommand(request), cancellationToken);
        return Created();
    }
}
