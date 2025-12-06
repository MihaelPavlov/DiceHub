using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Events.Commands;

public record ParticipateInEventCommand(int Id) : IRequest<bool>;

internal class ParticipateInEventCommandHandler : IRequestHandler<ParticipateInEventCommand, bool>
{
    readonly IRepository<Event> eventRepository;
    readonly IRepository<EventParticipant> eventParticipantRepository;
    readonly IUserContext userContext;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    readonly ILocalizationService localizer;

    public ParticipateInEventCommandHandler(
        IRepository<Event> eventRepository,
        IRepository<EventParticipant> eventParticipantRepository,
        IUserContext userContext, IStatisticQueuePublisher statisticQueuePublisher,
         ILocalizationService localizer)
    {
        this.eventRepository = eventRepository;
        this.eventParticipantRepository = eventParticipantRepository;
        this.userContext = userContext;
        this.statisticQueuePublisher = statisticQueuePublisher;
        this.localizer = localizer;
    }

    public async Task<bool> Handle(ParticipateInEventCommand request, CancellationToken cancellationToken)
    {
        var eventDb = await this.eventRepository
            .GetByAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.Id);

        var eventParticipants = await this.eventParticipantRepository
            .GetWithPropertiesAsync(x => x.EventId == request.Id, x => x, cancellationToken);

        var currentUserParticipant = await this.eventParticipantRepository
            .GetByAsync(x => x.EventId == request.Id && x.UserId == this.userContext.UserId, cancellationToken);

        if (eventDb.MaxPeople == eventParticipants.Count)
            throw new ValidationErrorsException("maxPeople", this.localizer["EventMaxPeopleCapacity"]);

        if (currentUserParticipant is null)
        {
            await this.eventParticipantRepository.AddAsync(new EventParticipant
            {
                EventId = eventDb.Id,
                UserId = this.userContext.UserId,
            }, cancellationToken);

            await this.statisticQueuePublisher.PublishAsync(new EventAttendanceDetectedJob(
               this.userContext.UserId, AttendanceAction.Joining, eventDb.Id, DateTime.UtcNow));

            return true;
        }

        return false;
    }
}