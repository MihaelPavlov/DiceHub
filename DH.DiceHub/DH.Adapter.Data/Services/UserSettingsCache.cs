using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantUserSettingsService;
using Microsoft.Extensions.Caching.Memory;

namespace DH.Adapter.Data.Services;

public class UserSettingsCache : IUserSettingsCache
{
    readonly IMemoryCache cache;
    readonly IRepository<TenantUserSetting> tenantUserSettingsRepository;

    public UserSettingsCache(IMemoryCache cache, IRepository<TenantUserSetting> tenantUserSettingsRepository)
    {
        this.cache = cache;
        this.tenantUserSettingsRepository = tenantUserSettingsRepository;
    }

    public async Task<string> GetLanguageAsync(string userId)
    {
        return await this.cache.GetOrCreateAsync($"lang:{userId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
            var settings = await this.tenantUserSettingsRepository.GetByAsync(x => x.UserId == userId, CancellationToken.None);
            return settings?.Language ?? SupportLanguages.EN.ToString();
        }) ?? SupportLanguages.EN.ToString();
    }

    public void InvalidateLanguage(string userId)
    {
        this.cache.Remove($"lang:{userId}");
    }
}