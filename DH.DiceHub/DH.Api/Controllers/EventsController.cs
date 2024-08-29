using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Commands.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Models.GameModels.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DH.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    readonly IMediator mediator;

    public EventsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    [ActionAuthorize(UserAction.EventsCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateGame([FromForm] string eventModel, [FromForm] IFormFile imageFile, CancellationToken cancellationToken)
    {
        var eventDto = JsonSerializer.Deserialize<CreateEventModel>(eventModel)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream, cancellationToken);

        var result = await this.mediator.Send(new CreateGameCommand(eventDto, imageFile.FileName, imageFile.ContentType, memoryStream), cancellationToken);
        return Ok(result);
    }

}
