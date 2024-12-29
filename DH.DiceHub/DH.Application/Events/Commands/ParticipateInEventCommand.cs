using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
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
    public ParticipateInEventCommandHandler(
        IRepository<Event> eventRepository,
        IRepository<EventParticipant> eventParticipantRepository,
        IUserContext userContext)
    {
        this.eventRepository = eventRepository;
        this.eventParticipantRepository = eventParticipantRepository;
        this.userContext = userContext;
    }

    public async Task<bool> Handle(ParticipateInEventCommand request, CancellationToken cancellationToken)
    {
        var eventDb = await this.eventRepository
            .GetByAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.Id);

        var currentUserParticipant = await this.eventParticipantRepository
            .GetByAsync(x => x.EventId == request.Id && x.UserId == this.userContext.UserId, cancellationToken);

        if (eventDb.MaxPeople == eventDb.Participants.Count)
            throw new ValidationErrorsException("maxPeople", "This event has reached its maximum capacity for online sign-ins");

        if (currentUserParticipant is null)
        {
            await this.eventParticipantRepository.AddAsync(new EventParticipant
            {
                EventId = eventDb.Id,
                UserId = this.userContext.UserId,
            }, cancellationToken);

            return true;
        }

        return false;
    }
}