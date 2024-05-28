using DH.Application.Cqrs;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using Mapster;

namespace DH.Application.Games.Commands;

public record CreateGameCommand(CreateGameDto Game) : ICommand<int>;

internal class CreateGameCommandHandler : AbstractCommandHandler<CreateGameCommand, int>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;

    public CreateGameCommandHandler(ITenantDbContext dbContext, IUserContext userContext)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
    }

    protected async override Task<int> HandleAsync(CreateGameCommand request, CancellationToken cancellationToken)
    {
        request.Game.UserId = userContext.UserId;

        if (!request.Game.FieldsAreValid(out var validationErrors))
            throw new ArgumentNullException(string.Join("\r\n", validationErrors));

        var gameRepository = dbContext.AcquireRepository<IRepository<Game>>();
        var game = await gameRepository.AddAsync(request.Game.Adapt<Game>(), cancellationToken);

        return game.Id;
    }
}
