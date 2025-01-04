using DH.OperationResultCore.Utility;
using DH.Statistics.Application.Commands;
using DH.Statistics.Application.Queries;
using DH.Statistics.Domain.Models.Queries;
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

    [HttpPost("get-activity-chart-data")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetActivityChartData>))]
    public async Task<IActionResult> GetActivityChartData([FromBody] GetActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
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

    [HttpPost("create-reservation-outcome-log")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> CreateReservationOutcomeLog([FromBody] CreateReservationOutcomeRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateReservationOutcomeCommand(request), cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-reservation-chart-data")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetReservationChartData>))]
    public async Task<IActionResult> GetReservationChartData([FromBody] GetReservationChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create-reward-history-log")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> CreateRewardHistoryLog([FromBody] CreateRewardHistoryLogRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateRewardHistoryLogCommand(request), cancellationToken);
        return Ok(result);
    }
}
