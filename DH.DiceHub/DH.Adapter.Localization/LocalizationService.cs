﻿using Microsoft.Extensions.Localization;

namespace DH.Domain.Adapters.Localization;

internal class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LocalizationService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string this[string key] => _localizer[key].Value;
}