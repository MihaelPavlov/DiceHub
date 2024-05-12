using DH.Application.Cqrs.GameCqrs;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserContext context;

    public GameController(IMediator mediator, IUserContext context)
    {
        _mediator = mediator;
        this.context = context;
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
