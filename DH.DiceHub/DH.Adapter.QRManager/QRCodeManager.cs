using DH.Adapter.QRManager.QRCodeStates;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.Domain.Services.Publisher;
using DH.OperationResultCore.Exceptions;
using System.Text.Json;

namespace DH.Adapter.QRManager;

/// <inheritdoc/>
public class QRCodeManager : IQRCodeManager
{
    readonly IQRCodeContext qRCodeContext;
    readonly IContainerService containerService;

    public QRCodeManager(IQRCodeContext qRCodeContext, IContainerService containerService)
    {
        this.qRCodeContext = qRCodeContext;
        this.containerService = containerService;
    }

    /// <inheritdoc/>
    public async Task<QrCodeValidationResult> ValidateQRCodeAsync(string data, CancellationToken cancellationToken)
    {
        var qrReader = this.ValidateCode(data);

        switch (qrReader.Type)
        {
            case QrCodeType.Game:
                this.qRCodeContext.SetState(
                    new GameQRCodeState(
                        this.containerService.Resolve<IRepository<Game>>()
                        )
                    );
                break;
            case QrCodeType.GameReservation:
                this.qRCodeContext.SetState(
                    new GameReservationQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IRepository<GameReservation>>(),
                        this.containerService.Resolve<IRepository<SpaceTableReservation>>(),
                        this.containerService.Resolve<IUserService>(),
                        this.containerService.Resolve<ISpaceTableService>(),
                        this.containerService.Resolve<SynchronizeGameSessionQueue>(),
                        this.containerService.Resolve<IRepository<Game>>(),
                        this.containerService.Resolve<IEventPublisherService>(),
                        this.containerService.Resolve<ReservationCleanupQueue>()
                        )
                    );
                break;
            case QrCodeType.Event:
                this.qRCodeContext.SetState(new EventQRCodeState());
                break;
            case QrCodeType.Reward:
                this.qRCodeContext.SetState(
                    new RewardQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IRepository<UserChallengeReward>>()
                        )
                    );
                break;
            case QrCodeType.TableReservation:
                this.qRCodeContext.SetState(
                    new TableReservationQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IRepository<SpaceTableReservation>>(),
                        this.containerService.Resolve<IEventPublisherService>()
                        )
                    );
                break;
            default:
                this.qRCodeContext.SetState(new UnknownQRCodeState());
                break;
        }

        return await this.qRCodeContext.HandleAsync(qrReader, cancellationToken);
    }

    /// <summary>
    /// Deserializes and validates the provided QR code data.
    /// Throws an exception if the data is invalid or malformed.
    /// </summary>
    /// <param name="data">The QR code data in JSON format.</param>
    /// <returns>Returns the validated <see cref="QRReaderModel"/>.</returns>
    /// <exception cref="BadRequestException">Thrown if the QR code data is invalid.</exception>
    /// <exception cref="JsonException">Thrown if the QR code format is incorrect.</exception>
    private QRReaderModel ValidateCode(string data)
    {
        try
        {
            var qrReader = JsonSerializer.Deserialize<QRReaderModel>(data);

            if (qrReader == null || qrReader.Id == 0 || string.IsNullOrEmpty(qrReader.Name) || !Enum.IsDefined(typeof(QrCodeType), qrReader.Type))
            {
                throw new BadRequestException("Invalid QR Code data.");
            }
            return qrReader;
        }
        catch (JsonException ex)
        {
            throw new JsonException("Invalid QR Code format.", ex);
        }
    }
}
