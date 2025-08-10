using DH.Adapter.Authentication.Filters;
using DH.Application.Rewards.Commands;
using DH.Application.Rewards.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Models.RewardModels.Commands;
using DH.Domain.Models.RewardModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RewardsController : ControllerBase
{
    readonly IMediator mediator;

    public RewardsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("system-reward/{id}")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSystemRewardByIdQueryModel))]
    public async Task<IActionResult> GetSystemRewardById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetSystemRewardByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("system-reward/get-dropdown-list")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSystemRewardByIdQueryModel>))]
    public async Task<IActionResult> GetSystemRewardsDropDownList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetSystemRewardDropdownListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-user-rewards")]
    [ActionAuthorize(UserAction.RewardsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserRewardListQueryModel>))]
    public async Task<IActionResult> GetUserRewardList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserRewardListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-user-period-rewards/{id}")]
    [ActionAuthorize(UserAction.RewardsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetUserChallengePeriodRewardListQueryModel>))]
    public async Task<IActionResult> GetUserChallengeRewardListByPeriodPerformanceId(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserChallengePeriodRewardListQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-all-system-rewards-list")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSystemRewardListQueryModel>))]
    public async Task<IActionResult> GetAllSystemRewardList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetAllSystemRewardListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("system-reward-list")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSystemRewardListQueryModel>))]
    public async Task<IActionResult> GetSystemRewardList([FromBody] GetSystemRewardListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("system-reward")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateSystemReward([FromForm] string reward, [FromForm] IFormFile imageFile, CancellationToken cancellationToken)
    {
        var rewardDto = JsonSerializer.Deserialize<CreateRewardDto>(reward)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream, cancellationToken);

        var result = await this.mediator.Send(new CreateSystemRewardCommand(
            rewardDto,
            imageFile.FileName,
            imageFile.ContentType,
            memoryStream
            ), cancellationToken);
        return Ok(result);
    }

    [HttpPut("system-reward")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSystemReward([FromForm] string reward, [FromForm] IFormFile? imageFile, CancellationToken cancellationToken)
    {
        var rewardDto = JsonSerializer.Deserialize<UpdateRewardDto>(reward)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();
        if (imageFile != null)
        {
            await imageFile.CopyToAsync(memoryStream, cancellationToken);
        }
        await this.mediator.Send(new UpdateSystemRewardCommand(
            rewardDto,
            imageFile?.FileName,
            imageFile?.ContentType,
            memoryStream
            ), cancellationToken);
        return Ok();
    }

    [HttpPost("user-reward-confirmation")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> UserRewardConfirmation([FromBody] UserRewardConfirmationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("system-reward/{id}")]
    [ActionAuthorize(UserAction.SystemRewardsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> DeleteSystemReward(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteSystemRewardCommand(id), cancellationToken);
        return Ok();
    }

    [HttpGet("get-image/{id}")]
    public async Task<IActionResult> GetRewardImage(int id, CancellationToken cancellationToken)
    {
        var rewardFile = await this.mediator.Send(new GetRewardImageByIdQuery(id), cancellationToken);
        if (rewardFile == null)
        {
            return NotFound();
        }

        return File(rewardFile.Data, rewardFile.ContentType, rewardFile.FileName);
    }
}
