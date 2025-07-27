using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Events.Commands;

public record SendEventParticipantsNotificationsCommand(int EventId) : IRequest;

internal class SendEventParticipantsNotificationsCommandHandler(
    ILogger<SendEventParticipantsNotificationsCommand> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IRepository<Event> eventRepository,
    IRepository<EventParticipant> eventParticipantsRepository,
    IPushNotificationsService pushNotificationsService,
    IUserContext userContext) : IRequestHandler<SendEventParticipantsNotificationsCommand>
{
    readonly ILogger<SendEventParticipantsNotificationsCommand> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IRepository<Event> eventRepository = eventRepository;
    readonly IRepository<EventParticipant> eventParticipantsRepository = eventParticipantsRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserContext userContext = userContext;

    public async Task Handle(SendEventParticipantsNotificationsCommand request, CancellationToken cancellationToken)
    {
        var eventDb = await this.eventRepository.GetByAsync(x => x.Id == request.EventId, cancellationToken);

        if (eventDb == null)
        {
            this.logger.LogWarning("Event with Id {Id} was not found. Event Notification was not send",
                request.EventId);
            throw new NotFoundException($"Event with Id {request.EventId} was not found");
        }

        //TODO: Send only to the users that have email notification enabled
        var participantIds = await this.eventParticipantsRepository.GetWithPropertiesAsync(
            x => x.EventId == eventDb.Id,
            x => x.UserId,
            cancellationToken);

        var (userLocalEventStartDate, isUtcFallback) =
                TimeZoneHelper.GetUserLocalOrUtcTime(eventDb.StartDate, this.userContext.TimeZone);

        if (isUtcFallback)
        {
            this.logger.LogWarning(
                "User local event date could not be calculated for event ID: {EventId}, time zone: {TimeZone}. Falling back to UTC.",
                eventDb.Id,
                this.userContext.TimeZone);
        }

        await this.pushNotificationsService
           .SendNotificationToUsersAsync(
               participantIds,
               new EventReminder(eventDb.Name, userLocalEventStartDate, isUtcFallback),
               cancellationToken);
    }
}