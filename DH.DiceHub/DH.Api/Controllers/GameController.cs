using DH.Application.Cqrs.GameCqrs;
using DH.Domain.Models.GameModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateGameDto game)
    {
        var cmd = new CreateGameCommand()
        {
            Game = game
        };

        var result = await _mediator.Send(cmd);

        return Ok(result);
    }
}
