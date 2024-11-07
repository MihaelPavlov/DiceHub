using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler(IGameService gameService, IUserContext userContext, ITenantSettingsCacheService tenantSettingsCacheService, IPushNotificationsService pushNotificationsService) : IRequestHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService = gameService;
    readonly IUserContext userContext = userContext;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListQuery request, CancellationToken cancellationToken)
    {
        await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        await this.pushNotificationsService.SendMessageAsync(new GameReservationReminder("Dungeon", DateTime.UtcNow) { DeviceToken= "c-svnhcnWcwOOVSzVdmO4u:APA91bGhnAs9AyJTfBfyF00l45SSK5l_JpA4ktDkZz77RfbjNCvLqXgTbwDbb0lHG1gxYJHbA5Ak1s0y-MaGodei16OgfkfZbaAooYUqWC9iLNBK2DhrosUY-LO8O_XBTiURtFHrqEAW" });
        return await gameService.GetGameListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
