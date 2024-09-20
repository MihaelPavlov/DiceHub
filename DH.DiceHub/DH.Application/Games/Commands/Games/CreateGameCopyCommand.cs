using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Commands.Games;

public class CreateGameCopyCommand : IRequest
{
    public int Id { get; set; }
    public string webRootPath { get; set; } = string.Empty;
}

internal class CreateGameCopyCommandHandler : IRequestHandler<CreateGameCopyCommand>
{
    readonly IRepository<Game> repository;
    readonly IRepository<GameInventory> gameInvetoryRepository;
    readonly IRepository<GameQrCode> gameQrCodesRepository;
    readonly IQRCodeManager qrCodeManager;

    public CreateGameCopyCommandHandler(
        IRepository<Game> repository,
        IRepository<GameInventory> gameInvetoryRepository,
        IRepository<GameQrCode> gameQrCodesRepository,
        IQRCodeManager qrCodeManager)
    {
        this.repository = repository;
        this.gameInvetoryRepository = gameInvetoryRepository;
        this.gameQrCodesRepository = gameQrCodesRepository;
        this.qrCodeManager = qrCodeManager;
    }

    public async Task Handle(CreateGameCopyCommand request, CancellationToken cancellationToken)
    {
        var game = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);

        var gameCopy = await this.gameInvetoryRepository.GetByAsyncWithTracking(x => x.GameId == game.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(GameInventory));

        gameCopy.TotalCopies++;
        gameCopy.AvailableCopies++;

        await this.repository.Update(game, cancellationToken);

        try
        {
            var fileName = this.qrCodeManager.CreateQRCode(new QRReaderModel
            {
                Id = game.Id,
                Name = game.Name,
                Type = QrCodeType.Game
            }, request.webRootPath);

            if (string.IsNullOrEmpty(fileName))
                throw new Exception("QR code was not successfully created");

            await this.gameQrCodesRepository.Update(new GameQrCode
            {
                GameId = game.Id,
                FileName = fileName,
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            //TODO: Handle the exception qrcode was not successufully 
        }
    }
}