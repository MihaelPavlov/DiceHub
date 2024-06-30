using DH.Application.Games.Commands;
using DH.Application.Games.Queries;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameList(GetGameListQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameByIdQueryModel))]
    public async Task<IActionResult> GetGameList(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateGameDto game, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateGameCommand(game), cancellationToken);

        return Ok(result);
    }
}
