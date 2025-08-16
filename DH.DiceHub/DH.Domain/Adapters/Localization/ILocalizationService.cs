namespace DH.Domain.Adapters.Localization;

public interface ILocalizationService
{
    string this[string key] { get; }

    /// <summary>
    /// Change the current culture for this request/context
    /// </summary>
    void SetLanguage(string cultureName);
}
