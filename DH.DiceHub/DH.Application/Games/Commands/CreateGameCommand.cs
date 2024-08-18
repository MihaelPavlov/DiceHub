using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Games.Commands;

public record CreateGameCommand(CreateGameDto Game) : IRequest<int>;

internal class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, int>
{
    readonly ITenantDbContext dbContext;
    readonly IUserContext userContext;

    public CreateGameCommandHandler(ITenantDbContext dbContext, IUserContext userContext)
    {
        this.dbContext = dbContext;
        this.userContext = userContext;
    }

    public async Task<int> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        request.Game.UserId = userContext.UserId;

        if (!request.Game.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var gameRepository = dbContext.AcquireRepository<IRepository<Game>>();
        var game = await gameRepository.AddAsync(request.Game.Adapt<Game>(), cancellationToken);

        return game.Id;
    }
}
