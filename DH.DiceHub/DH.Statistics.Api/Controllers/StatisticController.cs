using DH.OperationResultCore.Utility;
using DH.Statistics.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Statistics.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticController : ControllerBase
{
    readonly IMediator mediator;

    public StatisticController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("create-club-activity-log")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> CreateClubVisitorLog([FromBody] CreateClubVisitorLogRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateClubVisitorLogCommand(request), cancellationToken);
        return Ok(result);
    }
}
