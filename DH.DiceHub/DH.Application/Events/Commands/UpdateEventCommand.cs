using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
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
    readonly IUserManagementService userManagementService;
    readonly IPushNotificationsService pushNotificationsService;

    public UpdateEventCommandHandler(
        IEventService eventService,
        IPushNotificationsService pushNotificationsService,
        IUserManagementService userManagementService)
    {
        this.eventService = eventService;
        this.pushNotificationsService = pushNotificationsService;
        this.userManagementService = userManagementService;
    }

    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var response = await this.eventService.UpdateEvent(request.Event.Adapt<Event>(), request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken);

        if (response.ShouldSendStarDateUpdatedNotification)
        {
            var users = await this.userManagementService.GetUserListByRoles([Role.User, Role.Staff], cancellationToken);

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
