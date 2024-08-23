using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using Mapster;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record UpdateGameCommand(UpdateGameDto Game, string? FileName, string? ContentType, MemoryStream? ImageStream) : IRequest;

internal class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand>
{
    readonly IGameService gameService;
    readonly IRepository<Game> repository;
    public UpdateGameCommandHandler(IGameService gameService, IRepository<Game> repository)
    {
        this.gameService = gameService;
        this.repository = repository;
    }

    public async Task Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        if (!request.Game.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        if (request.Game.ImageId == null &&
            request.FileName != null &&
            request.ContentType != null &&
            request.ImageStream != null)
        {
            await this.gameService.UpdateGame(
                request.Game.Adapt<Game>(),
                request.FileName,
                request.ContentType,
                request.ImageStream,
                cancellationToken
            );

            return;
        }

        await this.repository.Update(request.Game.Adapt<Game>(), cancellationToken);
    }
}