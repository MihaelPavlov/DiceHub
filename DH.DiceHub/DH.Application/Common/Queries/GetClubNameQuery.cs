using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetClubNameQuery : IRequest<ClubNameResult>;

public class ClubNameResult
{
    public string ClubName { get; set; } = string.Empty;
    public string LogoFileName { get; set; } = string.Empty;
}

internal class GetClubNameQueryHandler : IRequestHandler<GetClubNameQuery, ClubNameResult>
{
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IRepository<Tenant> tenantRepository;
    readonly IUserContext userContext;

    public GetClubNameQueryHandler(
        ITenantSettingsCacheService tenantSettingsCacheService,
        IRepository<Tenant> tenantRepository,
        IUserContext userContext)
    {
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.tenantRepository = tenantRepository;
        this.userContext = userContext;
    }

    public async Task<ClubNameResult> Handle(GetClubNameQuery request, CancellationToken cancellationToken)
    {
        var settings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        var tenantId = this.userContext.TenantId == "system" ? null : this.userContext.TenantId;

        var logoFileName = string.Empty;
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            var tenant = await this.tenantRepository.GetByAsync(x => x.Id == tenantId, cancellationToken);
            logoFileName = tenant?.LogoFileName ?? string.Empty;
        }

        return new ClubNameResult
        {
            ClubName = settings.ClubName,
            LogoFileName = logoFileName,
        };
    }
}
