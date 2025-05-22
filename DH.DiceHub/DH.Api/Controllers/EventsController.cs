using DH.Adapter.Authentication.Filters;
using DH.Application.Events.Commands;
using DH.Application.Events.Queries;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Models.EventModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    readonly IMediator mediator;

    public EventsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("get-list-for-user")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetEventListQueryModel>))]
    public async Task<IActionResult> GetEventListForUser(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetEventListForUserQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-list-for-staff")]
    [ActionAuthorize(UserAction.EventsAdminRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetEventListQueryModel>))]
    public async Task<IActionResult> GetEventListForStaff([FromBody] GetEventListForStaffQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("send-event-notifications/{eventId}")]
    [ActionAuthorize(UserAction.EventsAdminRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEventNotifications(int eventId, CancellationToken cancellationToken)
    {
        await this.mediator.Send(
           new SendEventParticipantsNotificationsCommand(eventId), cancellationToken);
        return Ok();
    }

    [HttpGet("get-user-events")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetEventListQueryModel>))]
    public async Task<IActionResult> GetUserEvents(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetUserEventListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-all-events-dropdown-list")]
    [ActionAuthorize(UserAction.EventsAdminRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllEventsDropdownListModel>))]
    public async Task<IActionResult> GetAllEventsDropdownList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetAllEventsDropdownListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEventByIdQueryModel))]
    public async Task<IActionResult> GetEventById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetEventByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.EventsCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateEvent([FromForm] string eventModel, [FromForm] IFormFile? imageFile, CancellationToken cancellationToken)
    {
        var eventDto = JsonSerializer.Deserialize<CreateEventModel>(eventModel)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();

        if (eventDto.IsCustomImage && imageFile != null)
            await imageFile.CopyToAsync(memoryStream, cancellationToken);

        var result = await this.mediator.Send(new CreateEventCommand(eventDto, imageFile?.FileName, imageFile?.ContentType, memoryStream), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [ActionAuthorize(UserAction.EventsCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEvent([FromForm] string eventModel, [FromForm] IFormFile? imageFile, CancellationToken cancellationToken)
    {
        var eventDto = JsonSerializer.Deserialize<UpdateEventModel>(eventModel)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();

        if (eventDto.IsCustomImage && imageFile != null)
            await imageFile.CopyToAsync(memoryStream, cancellationToken);

        await this.mediator.Send(new UpdateEventCommand(eventDto, imageFile?.FileName, imageFile?.ContentType, memoryStream), cancellationToken);
        return Ok();
    }

    [HttpDelete("{eventId}")]
    [ActionAuthorize(UserAction.EventsCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEvent(int eventId, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteEventCommand(eventId), cancellationToken);
        return Ok();
    }

    [HttpPost("participate")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> ParticipateInEvent([FromBody] ParticipateInEventCommand command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("remove-participant")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> RemoveParticipantFromEvent([FromBody] RemoveParticipantFromEventCommand command, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("check-user-participation")]
    [ActionAuthorize(UserAction.EventsRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CheckUserParticipation([FromBody] CheckUserParticipationQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-image/{id}")]
    public async Task<IActionResult> GetEventImage(int id, CancellationToken cancellationToken)
    {
        var gameFile = await this.mediator.Send(new GetEventImageByIdQuery(id), cancellationToken);
        if (gameFile == null)
        {
            return NotFound();
        }

        return File(gameFile.Data, gameFile.ContentType, gameFile.FileName);
    }
}
