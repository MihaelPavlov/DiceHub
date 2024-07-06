using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Commands;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameReviewsController : ControllerBase
{
    readonly IMediator mediator;

    public GameReviewsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.GameReviewsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameByIdQueryModel))]
    public async Task<IActionResult> GetGameReviewList(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameReviewListQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.GameReviewsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateGameReview([FromBody] CreateGameReviewCommand request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}
