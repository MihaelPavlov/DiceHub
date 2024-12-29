using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Events.Commands;

public record CreateEventCommand(CreateEventModel Event, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest<int>;

internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    readonly IEventService eventService;

    public CreateEventCommandHandler(IEventService eventService)
    {
        this.eventService = eventService;
    }

    public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        if (!request.Event.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var eventId = await this.eventService.CreateEvent(
            request.Event.Adapt<Event>(),
            request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken
        );

        return eventId;
    }
}
