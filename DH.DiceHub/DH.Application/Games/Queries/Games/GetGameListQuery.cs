using DH.Domain.Adapters.Authentication;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameListQuery(string? SearchExpression) : IRequest<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler(IGameService gameService, IUserContext userContext, ITenantSettingsCacheService tenantSettingsCacheService) : IRequestHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IGameService gameService = gameService;
    readonly IUserContext userContext = userContext;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;

    public async Task<List<GetGameListQueryModel>> Handle(GetGameListQuery request, CancellationToken cancellationToken)
    {
        await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        return await gameService.GetGameListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, this.userContext.UserId, cancellationToken);
    }
}
