using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Commands.Games;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Models.GameModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    readonly IMediator mediator;

    public GamesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("list")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameList(GetGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-new-games")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetNewGameList(GetNewGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-games-by-category")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameListByCategoryId(GetGameListByCategoryIdQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameByIdQueryModel))]
    public async Task<IActionResult> GetGameById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameDto request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateGameCommand(request), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/like")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LikeGame(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new LikeGameCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPut("{id}/dislike")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DislikeGame(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DislikeGameCommand(id), cancellationToken);
        return Ok();
    }
}
