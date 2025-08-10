using DH.Domain.Adapters.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapater.Localization;

public static class DI
{
    public static IServiceCollection AddLocalizationAdapter(this IServiceCollection services)
    {
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddLocalization(options => options.ResourcesPath = "");
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "en", "bg" };
            options.SetDefaultCulture("en")
                   .AddSupportedCultures(supportedCultures)
                   .AddSupportedUICultures(supportedCultures);
        });

        return services;
    }
}