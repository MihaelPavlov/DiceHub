using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record LeaveSpaceTableCommand(int Id) : IRequest;

internal class LeaveSpaceTableCommandHandler : IRequestHandler<LeaveSpaceTableCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IUserContext userContext;
    readonly SynchronizeGameSessionQueue queue;
    readonly ILogger<LeaveSpaceTableCommandHandler> logger;

    public LeaveSpaceTableCommandHandler(IRepository<SpaceTable> spaceTableRepository, SynchronizeGameSessionQueue queue, ILogger<LeaveSpaceTableCommandHandler> logger, IUserContext userContext, IRepository<SpaceTableParticipant> spaceTableParticipantRepository)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.queue = queue;
        this.logger = logger;
        this.userContext = userContext;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
    }

    public async Task Handle(LeaveSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.Id && x.IsTableActive, cancellationToken)
                    ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        var spaceTableParticipation = await this.spaceTableParticipantRepository.GetByAsyncWithTracking(x => this.userContext.UserId == x.UserId && spaceTable.Id == x.SpaceTableId, cancellationToken);

        if (spaceTableParticipation == null)
        {
            var traceId = Guid.NewGuid().ToString();
            this.logger.LogWarning("{traceId}: User {userId} was not participation of the table {tableId}", traceId, spaceTable.Id, this.userContext.UserId);
            throw new InfrastructureException($"Something went wrong: reference traceId: {traceId}");
        }

        await this.spaceTableParticipantRepository.Remove(spaceTableParticipation, cancellationToken);

        if (this.queue.Contains(this.userContext.UserId, spaceTable.GameId))
            this.queue.CancelUserPlayTimeEnforcerJob(this.userContext.UserId, spaceTable.GameId);
    }
}