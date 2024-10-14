using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.SpaceManagementModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Commands;

public record CreateSpaceTableCommand(CreateSpaceTableDto SpaceTable) : IRequest<int>;

internal class CreateSpaceTableCommandHandler : IRequestHandler<CreateSpaceTableCommand, int>
{
    readonly ISpaceTableService spaceTableService;
    readonly IRepository<Game> gameRepostory;
    readonly IUserContext userContext;
    readonly ILogger<CreateSpaceTableCommand> logger;
    readonly SynchronizeGameSessionQueue queue;

    public CreateSpaceTableCommandHandler(ISpaceTableService spaceTableService, IRepository<Game> gameRepostory, SynchronizeGameSessionQueue queue, IUserContext userContext, ILogger<CreateSpaceTableCommand> logger)
    {
        this.spaceTableService = spaceTableService;
        this.queue = queue;
        this.gameRepostory = gameRepostory;
        this.queue = queue;
        this.userContext = userContext;
        this.logger = logger;
    }

    public async Task<int> Handle(CreateSpaceTableCommand request, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        if (!request.SpaceTable.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var spaceTableId = await this.spaceTableService.Create(request.SpaceTable.Adapt<SpaceTable>(), cancellationToken);

        var game = await this.gameRepostory.GetByAsync(x => x.Id == request.SpaceTable.GameId, cancellationToken);

        if (game == null)
        {
            this.logger.LogWarning("{traceId}: Table id-{spaceTableId}, was created. But AddUserPlayTimEnforcerJob was not added to the queue, because game with id-{gameId} was not founded", traceId, spaceTableId, request.SpaceTable.GameId);
            throw new InfrastructureException($"Something went wrong: reference traceId: {traceId}");
        }

        this.queue.AddUserPlayTimEnforcerJob(this.userContext.UserId, game!.Id, DateTime.UtcNow.AddSeconds(10));

        return spaceTableId;
    }
}