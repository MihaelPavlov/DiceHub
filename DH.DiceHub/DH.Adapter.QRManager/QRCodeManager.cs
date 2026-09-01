using DH.Adapter.QRManager.QRCodeStates;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DH.Adapter.QRManager;

/// <inheritdoc/>
public class QRCodeManager : IQRCodeManager
{
    readonly IQRCodeContext qRCodeContext;
    readonly IContainerService containerService;
    readonly IQrTokenService qrTokenService;

    public QRCodeManager(IQRCodeContext qRCodeContext, IContainerService containerService, IQrTokenService qrTokenService)
    {
        this.qRCodeContext = qRCodeContext;
        this.containerService = containerService;
        this.qrTokenService = qrTokenService;
    }

    /// <inheritdoc/>
    public async Task<QrCodeValidationResult> ValidateQRCodeAsync(string data, CancellationToken cancellationToken)
    {
        var isToken = false;
        QRReaderModel qrReader;

        if (LooksLikeToken(data))
        {
            qrReader = await this.qrTokenService.ResolveAsync(data, cancellationToken)
                ?? throw new BadRequestException("This QR code is invalid or has expired.");
            isToken = true;
        }
        else
        {
            // Legacy encrypted-JSON blob (QR codes printed before the token scheme).
            var decrypted = QrCodeDecryptor.Decrypt(data);
            qrReader = this.ValidateCode(decrypted);
        }

        switch (qrReader.Type)
        {
            case QrCodeType.Game:
                this.qRCodeContext.SetState(
                    new GameQRCodeState(
                        this.containerService.Resolve<IRepository<Game>>(),
                        this.containerService.Resolve<ILocalizationService>()
                        )
                    );
                break;
            case QrCodeType.GameReservation:
                this.qRCodeContext.SetState(
                    new GameReservationQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IRepository<GameReservation>>(),
                        this.containerService.Resolve<IRepository<SpaceTableReservation>>(),
                        this.containerService.Resolve<IUserManagementService>(),
                        this.containerService.Resolve<ISpaceTableService>(),
                        this.containerService.Resolve<IRepository<SpaceTable>>(),
                        this.containerService.Resolve<IGameSessionQueue>(),
                        this.containerService.Resolve<IRepository<Game>>(),
                        this.containerService.Resolve<IStatisticQueuePublisher>(),
                        this.containerService.Resolve<IReservationCleanupQueue>(),
                        this.containerService.Resolve<ILocalizationService>()
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
                        this.containerService.Resolve<IRepository<UserChallengeReward>>(),
                        this.containerService.Resolve<ILocalizationService>()
                        )
                    );
                break;
            case QrCodeType.TableReservation:
                this.qRCodeContext.SetState(
                    new TableReservationQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IRepository<SpaceTableReservation>>(),
                        this.containerService.Resolve<IRepository<SpaceTable>>(),
                        this.containerService.Resolve<IStatisticQueuePublisher>(),
                        this.containerService.Resolve<ILocalizationService>(),
                        this.containerService.Resolve<IReservationCleanupQueue>()
                        )
                    );
                break;
            case QrCodeType.PurchaseChallenge:
                this.qRCodeContext.SetState(
                    new PurchaseChallengeQRCodeState(
                        this.containerService.Resolve<IUserContext>(),
                        this.containerService.Resolve<IUniversalChallengeProcessing>(),
                        this.containerService.Resolve<ILocalizationService>()
                        )
                    );
                break;
            default:
                this.qRCodeContext.SetState(new UnknownQRCodeState());
                break;
        }

        var result = await this.qRCodeContext.HandleAsync(qrReader, cancellationToken);

        // Burn a per-user token once it has done its job so it can't be replayed.
        if (isToken && result.IsValid)
            await this.qrTokenService.MarkConsumedAsync(data, cancellationToken);

        return result;
    }

    /// <summary>
    /// Opaque token shape produced by <see cref="QrTokenService"/>: 1 type digit
    /// (1-6) followed by 11 uppercase base32 chars. Anything else is treated as a
    /// legacy encrypted blob.
    /// </summary>
    private static bool LooksLikeToken(string data)
    {
        if (string.IsNullOrEmpty(data) || data.Length != 12 || data[0] < '1' || data[0] > '6')
            return false;

        foreach (var c in data)
        {
            var ok = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z');
            if (!ok)
                return false;
        }

        return true;
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


public static class QrCodeDecryptor
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");

    public static string Decrypt(string encryptedBase64)
    {
        var fullCipher = Convert.FromBase64String(encryptedBase64);

        // First 16 bytes = IV
        var iv = new byte[16];
        var cipher = new byte[fullCipher.Length - 16];

        Array.Copy(fullCipher, 0, iv, 0, 16);
        Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}