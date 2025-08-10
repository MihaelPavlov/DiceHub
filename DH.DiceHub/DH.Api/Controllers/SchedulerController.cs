using DH.Adapter.Authentication.Filters;
using DH.Adapter.Scheduling.Jobs;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SchedulerController(
    ISchedulerService schedulerService, AddUserChallengePeriodJob addJob,
        UserChallengeValidationJob validationJob) : ControllerBase
{
    readonly ISchedulerService schedulerService = schedulerService;
    private readonly AddUserChallengePeriodJob _addJob = addJob;
    private readonly UserChallengeValidationJob _validationJob = validationJob;

    [HttpGet("get-schedule-jobs")]
    [ActionAuthorize(UserAction.SchedulerCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ScheduleJobInfo>))]
    public async Task<IActionResult> GetScheduleJobs(CancellationToken cancellationToken)
    {
        var result = await this.schedulerService.GetScheduleJobs();
        return Ok(result);
    }

    [HttpGet("run-concurrent")]
    public async Task<IActionResult> RunBothJobsConcurrently()
    {
        await _addJob.Execute(null); // First job

        await Task.Delay(TimeSpan.FromSeconds(10)); // Delay between jobs

        await _validationJob.Execute(null); // Second job

        return Ok("First job completed, then second job ran after delay.");
    }
}
