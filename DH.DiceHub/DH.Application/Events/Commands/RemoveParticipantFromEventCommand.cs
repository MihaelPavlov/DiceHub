using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Events.Commands;

public record RemoveParticipantFromEventCommand(int Id) : IRequest<bool>;

internal class RemoveParticipantFromEventCommandHandler : IRequestHandler<RemoveParticipantFromEventCommand, bool>
{
    readonly IRepository<Event> eventRepository;
    readonly IRepository<EventParticipant> eventParticipantRepository;
    readonly IUserContext userContext;
    public RemoveParticipantFromEventCommandHandler(
        IRepository<Event> eventRepository,
        IRepository<EventParticipant> eventParticipantRepository,
        IUserContext userContext)
    {
        this.eventRepository = eventRepository;
        this.eventParticipantRepository = eventParticipantRepository;
        this.userContext = userContext;
    }

    public async Task<bool> Handle(RemoveParticipantFromEventCommand request, CancellationToken cancellationToken)
    {
        var eventDb = await this.eventRepository
            .GetByAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Event), request.Id);

        var currentUserParticipant = await this.eventParticipantRepository
            .GetByAsync(x => x.EventId == request.Id && x.UserId == this.userContext.UserId, cancellationToken);

        if (currentUserParticipant is not null)
        {
            await this.eventParticipantRepository.Remove(currentUserParticipant, cancellationToken);

            return true;
        }

        return false;
    }
}

