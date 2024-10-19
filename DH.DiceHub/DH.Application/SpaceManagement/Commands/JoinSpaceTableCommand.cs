using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record JoinSpaceTableCommand(int Id, string Password) : IRequest;

internal class JoinSpaceTableCommandHandler : IRequestHandler<JoinSpaceTableCommand>
{
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IRepository<Game> gameRepostory;
    readonly IUserContext userContext;
    readonly SynchronizeGameSessionQueue queue;
    readonly ILogger<JoinSpaceTableCommandHandler> logger;

    public JoinSpaceTableCommandHandler(
        IRepository<SpaceTable> spaceTableRepository,
        IRepository<SpaceTableParticipant> spaceTableParticipantRepository,
        IRepository<Game> gameRepostory,
        IUserContext userContext,
        SynchronizeGameSessionQueue queue,
        ILogger<JoinSpaceTableCommandHandler> logger)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.gameRepostory = gameRepostory;
        this.userContext = userContext;
        this.queue = queue;
        this.logger = logger;
    }

    public async Task Handle(JoinSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        if (spaceTable.IsLocked && !spaceTable.Password.Equals(request.Password))
            throw new ValidationErrorsException("Password", "Password doesn't match");

        var spaceTableParticipations = await this.spaceTableParticipantRepository.GetWithPropertiesAsTrackingAsync(x => x.SpaceTableId == spaceTable.Id, x => x, cancellationToken);

        if (spaceTableParticipations.Count >= spaceTable.MaxPeople)
            throw new ValidationErrorsException("MaxPeople", "Max People was reached for this room");

        spaceTable.SpaceTableParticipants.Add(new SpaceTableParticipant
        {
            SpaceTableId = spaceTable.Id,
            UserId = this.userContext.UserId,
            JoinedAt = DateTime.UtcNow,
        });

        await this.spaceTableRepository.SaveChangesAsync(cancellationToken);

        var traceId = Guid.NewGuid().ToString();
        var game = await this.gameRepostory.GetByAsync(x => x.Id == spaceTable.GameId, cancellationToken);

        if (game == null)
        {
            this.logger.LogWarning("{traceId}: Table Participation was added to table id-{spaceTableId}. But AddUserPlayTimEnforcerJob was not added to the queue, because game with id-{gameId} was not founded", traceId, spaceTable.Id, spaceTable.GameId);
            throw new InfrastructureException($"Something went wrong: reference traceId: {traceId}");
        }

        this.queue.AddUserPlayTimEnforcerJob(this.userContext.UserId, game!.Id, DateTime.UtcNow.AddMinutes((int)game.AveragePlaytime));
    }
}