using DH.Adapter.QRManager.QRCodeStates;
using DH.Domain;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using DH.Domain.Services;
using SkiaSharp;
using System.Text.Json;
using ZXing.QrCode;

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
    public string CreateQRCode(QRReaderModel data, string webRootPath)
    {
        var writer = new QRCodeWriter();
        var resultBit = writer.encode(JsonSerializer.Serialize(data), ZXing.BarcodeFormat.QR_CODE, 200, 200);
        var matrix = resultBit;

        int scale = 2;
        int width = matrix.Width * scale;
        int height = matrix.Height * scale;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Black;

                for (int x = 0; x < matrix.Height; x++)
                {
                    for (int y = 0; y < matrix.Width; y++)
                    {
                        if (matrix[x, y])
                        {
                            canvas.DrawRect(x * scale, y * scale, scale, scale, paint);
                        }
                    }
                }
            }

            using (var image = surface.Snapshot())
            using (var dataImage = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                var baseDirectory = Path.Combine(webRootPath, "images");
                string directory = baseDirectory;
                string fileName = string.Empty;

                var currentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                switch (data.Type)
                {
                    case QrCodeType.Game:
                        directory = Path.Combine(baseDirectory, "games");
                        fileName = $"{data.Id}_{currentDate}.png";
                        break;
                    case QrCodeType.Event:
                        directory = Path.Combine(baseDirectory, "events");
                        fileName = $"{data.Id}_{currentDate}.png";
                        break;
                    default:
                        /*
                        TODO: Currently we are not handling the unknown type
                        We throw exception and visualzie it in the UI
                        */
                        return string.Empty;
                }

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var filePath = Path.Combine(directory, fileName);

                using (var stream = File.OpenWrite(filePath))
                {
                    dataImage.SaveTo(stream);
                }

                return fileName;
            }
        }
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
                        this.containerService.Resolve<IUserService>(),
                        this.containerService.Resolve<ISpaceTableService>(),
                        this.containerService.Resolve<SynchronizeGameSessionQueue>(),
                        this.containerService.Resolve<IJobManager>(),
                        this.containerService.Resolve<IRepository<Game>>()
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
                        this.containerService.Resolve<IRepository<SpaceTableReservation>>()
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
