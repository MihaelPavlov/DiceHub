using DH.Adapter.Authentication.Filters;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SchedulerController(
    ISchedulerService schedulerService) : ControllerBase
{
    readonly ISchedulerService schedulerService = schedulerService;

    [HttpGet("get-schedule-jobs")]
    [ActionAuthorize(UserAction.SchedulerCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ScheduleJobInfo>))]
    public async Task<IActionResult> GetScheduleJobs(CancellationToken cancellationToken)
    {
        var result = await this.schedulerService.GetScheduleJobs();
        return Ok(result);
    }
}
