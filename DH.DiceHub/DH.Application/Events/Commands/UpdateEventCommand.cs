using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Models.EventModels.Command;
using DH.Domain.Services;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Events.Commands;

public record UpdateEventCommand(UpdateEventModel Event, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest;

internal class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
    readonly IEventService eventService;
    readonly IUserService userService;
    readonly IPushNotificationsService pushNotificationsService;
    readonly ILogger<UpdateEventCommandHandler> logger;
    readonly IUserContext userContext;
    public UpdateEventCommandHandler(
        IEventService eventService, IPushNotificationsService pushNotificationsService,
        ILogger<UpdateEventCommandHandler> logger, IUserContext userContext,
        IUserService userService)
    {
        this.eventService = eventService;
        this.pushNotificationsService = pushNotificationsService;
        this.logger = logger;
        this.userContext = userContext;
        this.userService = userService;
    }

    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var response = await this.eventService.UpdateEvent(request.Event.Adapt<Event>(), request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken);

        if (response.ShouldSendStarDateUpdatedNotification)
        {
            var users = await this.userService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);

            var userIds = users.Select(x => x.Id).ToList();

            var payload = new EventStartDateUpdatedNotification
            {
                EventDate = request.Event.StartDate,
                EventName = request.Event.Name,
            };

            await this.pushNotificationsService
               .SendNotificationToUsersAsync(
                   userIds, payload, cancellationToken);
        }
    }
}
