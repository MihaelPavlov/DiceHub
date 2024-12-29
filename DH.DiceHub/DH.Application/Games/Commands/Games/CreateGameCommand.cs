using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public record CreateGameCommand(CreateGameDto Game, string FileName, string ContentType, MemoryStream ImageStream, string webRootPath) : IRequest<int>;

internal class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, int>
{
    readonly IGameService gameService;
    readonly IRepository<GameQrCode> gameQrCodesRepository;
    readonly IQRCodeManager qrCodeManager;

    public CreateGameCommandHandler(IGameService gameService, IRepository<GameQrCode> gameQrCodesRepository, IQRCodeManager qrCodeManager)
    {
        this.gameService = gameService;
        this.gameQrCodesRepository = gameQrCodesRepository;
        this.qrCodeManager = qrCodeManager;
    }

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

        try
        {
            var fileName = this.qrCodeManager.CreateQRCode(new QRReaderModel
            {
                Id = gameId,
                Name = request.Game.Name,
                Type = QrCodeType.Game
            }, request.webRootPath);

            if (string.IsNullOrEmpty(fileName))
                throw new Exception("QR code was not successfully created");

            await this.gameQrCodesRepository.Update(new GameQrCode
            {
                GameId = gameId,
                FileName = fileName,
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            //TODO: Handle the exception qrcode was not successufully 
        }

        return gameId;
    }
}
