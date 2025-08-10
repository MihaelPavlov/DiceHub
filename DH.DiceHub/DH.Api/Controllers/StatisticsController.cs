using DH.Application.Statistics.Queries;
using DH.Domain.Adapters.Statistics.JobHandlers;
using DH.Domain.Models.StatisticsModels.Queries;
using DH.OperationResultCore.Utility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    readonly IMediator mediator;

    public StatisticsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("get-activity-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetActivityChartData>))]
    public async Task<IActionResult> GetActivityChartData([FromBody] GetActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-event-attendance-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetEventAttendanceChartData>))]
    public async Task<IActionResult> GetEventAttendanceChartData([FromBody] GetEventAttendanceChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-event-attendance-by-ids")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetEventAttendanceChartData>))]
    public async Task<IActionResult> GetEventAttendanceByIds([FromBody] GetEventAttendanceByIdsQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }


    [HttpPost("get-challenge-history-log")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<List<GetChallengeHistoryLogQueryResponse>>))]
    public async Task<IActionResult> GetChallengeHistoryLogs([FromBody] GetChallengeHistoryLogQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-reservation-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetReservationChartData>))]
    public async Task<IActionResult> GetReservationChartData([FromBody] GetReservationChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-collected-rewards-by-dates")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<List<GetCollectedRewardByDatesModel>>))]
    public async Task<IActionResult> GetCollectedRewardsByDates([FromBody] GetCollectedRewardsByDatesQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-expired-collected-rewards-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetExpiredCollectedRewardsChartDataModel>))]
    public async Task<IActionResult> GetExpiredCollectedRewardsChartData([FromBody] GetExpiredCollectedRewardsChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-game-engagement-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetGameActivityChartData>))]
    public async Task<IActionResult> GetGameEngagementChartData([FromBody] GetGameActivityChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-game-user-engagement-chart-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<GetUsersWhoPlayedGameData>))]
    public async Task<IActionResult> GetGameUserEngagementChartData([FromBody] GetUserWhoPlayedGameChartDataQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
