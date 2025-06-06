﻿using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameQRCodeState : IQRCodeState
{
    readonly IRepository<Game> gameRepository;

    public GameQRCodeState(IRepository<Game> gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();
        var validationResult = new QrCodeValidationResult(data.Id, QrCodeType.Game);
        try
        {
            var game = await this.gameRepository.GetByAsyncWithTracking(x => x.Id == data.Id, cancellationToken);

            if (game == null)
            {
                validationResult.ErrorMessage = "Game doesn't exists. Reach someone from the staff with the scanned game.";
                await context.TrackScannedQrCode(traceId, data, new NotFoundException(validationResult.ErrorMessage), cancellationToken);
                return validationResult;
            }

            await context.TrackScannedQrCode(traceId, data, null, cancellationToken);
            validationResult.IsValid = true;
        }
        catch (Exception ex)
        {
            validationResult.IsValid = false;
            validationResult.ErrorMessage = "Something whent wrong. Contact staff";
            await context.TrackScannedQrCode(traceId, data, ex, cancellationToken);
        }

        return validationResult;
    }
}
