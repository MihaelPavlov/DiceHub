using DH.Adapter.QRManager.QRCodeStates;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using SkiaSharp;
using System.Text.Json;
using ZXing.QrCode;

namespace DH.Adapter.QRManager;

internal class QRCodeManager : IQRCodeManager
{
    readonly IQRCodeContext qRCodeContext;

    public QRCodeManager(IQRCodeContext qRCodeContext)
    {
        this.qRCodeContext = qRCodeContext;
    }

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

    public async Task ProcessQRCodeAsync(string data, CancellationToken cancellationToken)
    {
        this.ValidateCode(data);
        var codeType = this.IdentifyQrCodeType(data);

        switch (codeType)
        {
            case QrCodeType.Game:
                this.qRCodeContext.SetState(new GameQRCodeState());
                break;
            case QrCodeType.Event:
                this.qRCodeContext.SetState(new EventQRCodeState());
                break;
            default:
                this.qRCodeContext.SetState(new UnknownQRCodeState());
                break;
        }

        await this.qRCodeContext.HandleAsync(data, cancellationToken);
    }

    public QrCodeType IdentifyQrCodeType(string data)
    {
        var qrReader = JsonSerializer.Deserialize<QRReaderModel>(data);

        if (qrReader != null)
            return qrReader.Type;

        return QrCodeType.Unknown;
    }

    public void ValidateCode(string data)
    {
        var qrReader = JsonSerializer.Deserialize<QRReaderModel>(data);

        if (qrReader != null &&
            qrReader.Id != 0 &&
            !string.IsNullOrEmpty(qrReader.Name) &&
            Enum.IsDefined(qrReader.Type))
            return;


        throw new Exception("Invalid QR Code");
    }
}
