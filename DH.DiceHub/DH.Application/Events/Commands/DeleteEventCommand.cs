using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Events.Commands;

public record DeleteEventCommand(int EventId) : IRequest;

internal class DeleteEventCommandHandler(
    IRepository<Event> eventRepository,
    IRepository<EventParticipant> eventParticipantRepository,
    IPushNotificationsService pushNotificationsService,
    IUserContext userContext,
    ILogger<DeleteEventCommand> logger) : IRequestHandler<DeleteEventCommand>
{
    readonly IRepository<Event> eventRepository = eventRepository;
    readonly IRepository<EventParticipant> eventParticipantRepository = eventParticipantRepository;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly IUserContext userContext = userContext;
    readonly ILogger<DeleteEventCommand> logger = logger;

    public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventDb = await this.eventRepository.GetByAsyncWithTracking(x => x.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException($"Event with id {request.EventId} not found");

        eventDb.IsDeleted = true;

        var eventParticipants = await this.eventParticipantRepository
            .GetWithPropertiesAsync(x => x.EventId == request.EventId, x => x, cancellationToken) ?? [];

        if (eventParticipants.Count != 0)
        {
            await this.eventParticipantRepository.RemoveRange(eventParticipants, cancellationToken);

            var userIds = eventParticipants.Select(x => x.UserId).ToList();

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
                    userIds,
                    new EventDeletedMessage(eventDb.Name, userLocalEventStartDate, isUtcFallback),
                    cancellationToken);

            return;
        }

        await this.eventRepository.SaveChangesAsync(cancellationToken);
    }
}