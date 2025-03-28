﻿using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record CreateGameCommand(CreateGameDto Game, string FileName, string ContentType, MemoryStream ImageStream) : IRequest<int>;

internal class CreateGameCommandHandler(IGameService gameService) : IRequestHandler<CreateGameCommand, int>
{
    readonly IGameService gameService = gameService;

    public async Task<int> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        if (!request.Game.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var gameId = await this.gameService.CreateGame(
            request.Game.Adapt<Game>(),
            request.FileName,
            request.ContentType,
            request.ImageStream,
            cancellationToken
        );

        return gameId;
    }
}
