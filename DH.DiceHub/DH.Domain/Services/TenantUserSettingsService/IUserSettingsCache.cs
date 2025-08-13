namespace DH.Domain.Services.TenantUserSettingsService;

public interface IUserSettingsCache
{
    Task<string> GetLanguageAsync(string userId);
    void InvalidateLanguage(string userId);
}