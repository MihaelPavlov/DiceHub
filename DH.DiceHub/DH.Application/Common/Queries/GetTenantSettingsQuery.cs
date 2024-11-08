using DH.Domain.Entities;
using DH.Domain.Services.TenantSettingsService;
using Mapster;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetTenantSettingsQuery : IRequest<TenantSettingDto>;

internal class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, TenantSettingDto>
{
    readonly ITenantSettingsCacheService tenantSettingsCacheService;

    public GetTenantSettingsQueryHandler(ITenantSettingsCacheService tenantSettingsCacheService)
    {
        this.tenantSettingsCacheService = tenantSettingsCacheService;
    }

    public async Task<TenantSettingDto> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        return settings.Adapt<TenantSettingDto>();
    }
}