using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;

namespace DH.Domain.Adapters.PushNotifications;

public class NotificationRenderer(
    ILocalizationService localizer, IRepository<TenantUserSetting> tenantUserRepository,
    IUserService userService) : INotificationRenderer
{
    readonly ILocalizationService localizer = localizer;
    readonly IRepository<TenantUserSetting> tenantUserRepository = tenantUserRepository;
    readonly IUserService userService = userService;

    public async Task<NotificationPayload?> RenderMessageBody<TPayload>(TPayload payload, string userId) where TPayload : RenderableNotification
    {
        var tenantUserSetting = await this.tenantUserRepository.GetByAsync(x => x.UserId == userId, CancellationToken.None);

        var language = tenantUserSetting == null || string.IsNullOrEmpty(tenantUserSetting.Language)
            ? SupportLanguages.EN.ToString()
            : tenantUserSetting.Language;

        this.localizer.SetLanguage(language);

        var userTimeZone = await this.userService.GetUserTimeZone(userId);
        var userLanguage = language;

        if (payload is RenderableNotification renderable)
        {
            return renderable.Render(localizer, userTimeZone, userLanguage);
        }
        return null;
    }
}
