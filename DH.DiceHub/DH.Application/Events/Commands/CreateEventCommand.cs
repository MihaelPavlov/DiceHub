using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Events.Commands;

public record CreateEventCommand(
    CreateEventModel Event,
    string? FileName,
    string? ContentType,
    MemoryStream? ImageStream) : IRequest<int>;

internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    readonly IEventService eventService;
    readonly IUserService userService;
    readonly IPushNotificationsService pushNotificationsService;

    public CreateEventCommandHandler(
        IEventService eventService,
        IUserService userService,
        IPushNotificationsService pushNotificationsService)
    {
        this.eventService = eventService;
        this.userService = userService;
        this.pushNotificationsService = pushNotificationsService;
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

        var users = await this.userService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);

        await this.pushNotificationsService
           .SendNotificationToUsersAsync(
               users,
               new NewEventAddedMessage(
                request.Event.Name,
                request.Event.StartDate.ToLocalTime()),
               cancellationToken);

        return eventId;
    }
}
