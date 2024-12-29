using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services.Publisher;
using DH.Messaging.Publisher.Messages;
using DH.OperationResultCore.Exceptions;
using MediatR;
using System.ComponentModel.Design;

namespace DH.Application.Events.Commands;

public record RemoveParticipantFromEventCommand(int Id) : IRequest<bool>;

internal class RemoveParticipantFromEventCommandHandler : IRequestHandler<RemoveParticipantFromEventCommand, bool>
{
    readonly IRepository<Event> eventRepository;
    readonly IRepository<EventParticipant> eventParticipantRepository;
    readonly IUserContext userContext;
    readonly IEventPublisherService eventPublisherService;
    public RemoveParticipantFromEventCommandHandler(
        IRepository<Event> eventRepository,
        IRepository<EventParticipant> eventParticipantRepository,
        IUserContext userContext,
        IEventPublisherService eventPublisherService)
    {
        this.eventRepository = eventRepository;
        this.eventParticipantRepository = eventParticipantRepository;
        this.userContext = userContext;
        this.eventPublisherService = eventPublisherService;
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

            await this.eventPublisherService.PublishEventAttendanceDetectedMessage(AttendanceAction.Leaving.ToString(), eventDb.Id);

            return true;
        }

        return false;
    }
}

