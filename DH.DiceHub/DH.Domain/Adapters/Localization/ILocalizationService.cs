namespace DH.Domain.Adapters.Localization;

public interface ILocalizationService
{
    string this[string key] { get; }
}
