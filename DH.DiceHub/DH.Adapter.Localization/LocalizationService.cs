using Microsoft.Extensions.Localization;
using System.Globalization;

namespace DH.Domain.Adapters.Localization;

internal class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LocalizationService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string this[string key] => _localizer[key].Value;

    /// <inheritdoc/>
    public void SetLanguage(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return;

        var culture = new CultureInfo(cultureName);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
}