using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Helpers;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

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
    readonly ILogger<CreateEventCommandHandler> logger;
    readonly IUserContext userContext;
    public CreateEventCommandHandler(
        IEventService eventService,
        IUserService userService,
        IPushNotificationsService pushNotificationsService,
        IUserContext userContext,
        ILogger<CreateEventCommandHandler> logger)
    {
        this.eventService = eventService;
        this.userService = userService;
        this.pushNotificationsService = pushNotificationsService;
        this.logger = logger;
        this.userContext = userContext;
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

        //TODO: Send to all staff and only to the users that have email notification enabled
        var users = await this.userService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);

        var userIds = users.Select(x => x.Id).ToList();


        var (userLocalEventStartDate, isUtcFallback) =
            TimeZoneHelper.GetUserLocalOrUtcTime(request.Event.StartDate, this.userContext.TimeZone);

        if (isUtcFallback)
        {
            this.logger.LogWarning(
                "User local event date could not be calculated for event ID: {EventId}, time zone: {TimeZone}. Falling back to UTC.",
                eventId,
                this.userContext.TimeZone);
        }

        await this.pushNotificationsService
           .SendNotificationToUsersAsync(
               userIds,
               new NewEventAddedMessage(
                request.Event.Name,
                userLocalEventStartDate,
                isUtcFallback),
               cancellationToken);

        return eventId;
    }
}
