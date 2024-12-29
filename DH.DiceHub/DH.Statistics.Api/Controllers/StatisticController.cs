using DH.OperationResultCore.Utility;
using DH.Statistics.Application.Commands;
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

    [HttpPost("create-event-attendance-log")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> CreateEventAttendanceLog([FromBody] CreateEventAttendanceLogRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateEventAttendanceLogCommand(request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("remove-event-attendance-log")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> RemoveEventAttendanceLog([FromBody] RemoveEventAttendanceLogRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new RemoveEventAttendanceLogCommand(request), cancellationToken);
        return Ok(result);
    }
}
