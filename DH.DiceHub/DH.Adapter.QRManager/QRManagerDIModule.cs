using DH.Domain.Adapters.QRManager;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.QRManager;

public static class QRManagerDIModule
{
    public static void ConfigureQrCodeManager(
        this IServiceCollection services)
        => services
            .AddScoped<IQRCodeManager, QRCodeManager>()
            .AddSingleton<IQRCodeContext, QRCodeContext>();
}
