using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Services;
using Mapster;
using MediatR;

namespace DH.Application.Events.Commands;

public record UpdateEventCommand(UpdateEventModel Event, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest;

internal class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
    readonly IEventService eventService;

    public UpdateEventCommandHandler(IEventService eventService)
    {
        this.eventService = eventService;
    }

    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        await this.eventService.UpdateEvent(request.Event.Adapt<Event>(), request.FileName,
           request.ContentType,
           request.ImageStream,
           cancellationToken);
    }
}
