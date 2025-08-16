using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
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
    readonly ILocalizationService localizer;

    public CreateEventCommandHandler(
        IEventService eventService,
        IUserService userService,
        IPushNotificationsService pushNotificationsService,
        ILocalizationService localizer)
    {
        this.eventService = eventService;
        this.userService = userService;
        this.pushNotificationsService = pushNotificationsService;
        this.localizer = localizer;
    }

    public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        if (!request.Event.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var eventId = await this.eventService.CreateEvent(
            request.Event.Adapt<Event>(),
            request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken
        );

        //TODO: Send to all staff and only to the users that have email notification enabled
        var users = await this.userService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);

        var userIds = users.Select(x => x.Id).ToList();

        var payload = new NewEventAddedNotification
        {
            EventName = request.Event.Name,
            EventDate = request.Event.StartDate
        };

        await this.pushNotificationsService
           .SendNotificationToUsersAsync(userIds, payload, cancellationToken);

        return eventId;
    }
}
