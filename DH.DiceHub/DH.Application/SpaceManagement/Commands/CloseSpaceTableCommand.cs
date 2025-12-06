using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record CloseSpaceTableCommand(int Id) : IRequest;

internal class CloseSpaceTableCommandHandler : IRequestHandler<CloseSpaceTableCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IRepository<GameInventory> gameInventoryRepository;
    readonly IGameSessionQueue queue;
    readonly ILogger<CloseSpaceTableCommandHandler> logger;

    public CloseSpaceTableCommandHandler(
        IRepository<SpaceTable> spaceTableRepository, IGameSessionQueue queue,
        ILogger<CloseSpaceTableCommandHandler> logger, IRepository<GameInventory> gameInventoryRepository,
        IRepository<SpaceTableParticipant> spaceTableParticipantRepository)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.queue = queue;
        this.logger = logger;
        this.gameInventoryRepository = gameInventoryRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
    }

    public async Task Handle(CloseSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.Id && x.IsTableActive, cancellationToken)
                    ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        var spaceTableParticipantList = await this.spaceTableParticipantRepository.GetWithPropertiesAsync(x => x.SpaceTableId == spaceTable.Id, x => x, cancellationToken);

        foreach (var participant in spaceTableParticipantList)
        {
            if (await this.queue.Contains(participant.UserId, spaceTable.GameId, cancellationToken))
                await this.queue.CancelUserPlayTimeEnforcerJob(participant.UserId, spaceTable.GameId);
        }

        if (await this.queue.Contains(spaceTable.CreatedBy, spaceTable.GameId, cancellationToken))
            await this.queue.CancelUserPlayTimeEnforcerJob(spaceTable.CreatedBy, spaceTable.GameId);

        await this.spaceTableParticipantRepository.RemoveRange(spaceTableParticipantList, cancellationToken);

        spaceTable.IsTableActive = false;

        var gameInventory = await this.gameInventoryRepository.GetByAsyncWithTracking(x => x.GameId == spaceTable.GameId, cancellationToken);
        if (gameInventory == null)
        {
            var traceId = Guid.NewGuid().ToString();
            this.logger.LogWarning("{traceId}: Game Inventory was not found. Game Id -> {gameId}", traceId, spaceTable.GameId);
            throw new InfrastructureException($"Something went wrong: reference traceId: {traceId}");
        }

        gameInventory.AvailableCopies++;
        await this.spaceTableRepository.SaveChangesAsync(cancellationToken);
    }
}
