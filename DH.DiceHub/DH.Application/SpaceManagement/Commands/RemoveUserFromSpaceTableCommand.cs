using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record RemoveUserFromSpaceTableCommand(int Id, string UserId) : IRequest;

internal class RemoveUserFromSpaceTableCommandHandler : IRequestHandler<RemoveUserFromSpaceTableCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly SynchronizeGameSessionQueue queue;
    readonly IUserContext userContext;

    public RemoveUserFromSpaceTableCommandHandler(IRepository<SpaceTable> spaceTableRepository, SynchronizeGameSessionQueue queue, IRepository<SpaceTableParticipant> spaceTableParticipantRepository, IUserContext userContext)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.queue = queue;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.userContext = userContext;
    }

    public async Task Handle(RemoveUserFromSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.Id && x.IsTableActive, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        if (this.userContext.UserId != spaceTable.CreatedBy)
            throw new BadRequestException("Only creator of the table can remove participants from it");

        var spaceTableParticipation = await this.spaceTableParticipantRepository.GetByAsyncWithTracking(x => request.UserId == x.UserId && spaceTable.Id == x.SpaceTableId, cancellationToken);

        // If not found means that the participant already left the space table: just return
        if (spaceTableParticipation == null)
            return;

        await this.spaceTableParticipantRepository.Remove(spaceTableParticipation, cancellationToken);

        if (this.queue.Contains(request.UserId, spaceTable.GameId))
            this.queue.CancelUserPlayTimeEnforcerJob(request.UserId, spaceTable.GameId);
    }
}