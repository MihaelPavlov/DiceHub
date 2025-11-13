using DH.Domain.Services.TenantSettingsService;
using MediatR;

namespace DH.Application.Common.Queries;

public record GetClubInfoQuery : IRequest<GetClubInfoModel>;

internal class GetClubInfoQueryHandler : IRequestHandler<GetClubInfoQuery, GetClubInfoModel>
{
    readonly ITenantSettingsCacheService tenantSettingsCacheService;

    public GetClubInfoQueryHandler(ITenantSettingsCacheService tenantSettingsCacheService)
    {
        this.tenantSettingsCacheService = tenantSettingsCacheService;
    }

    public async Task<GetClubInfoModel> Handle(GetClubInfoQuery request, CancellationToken cancellationToken)
    {
        var settings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        return new GetClubInfoModel
        {
            DaysOff = string.IsNullOrEmpty(settings.DaysOff) ? [] : settings.DaysOff.Split(",").OrderBy(x => x).ToList(),
            StartWorkingHours = settings.StartWorkingHours,
            EndWorkingHours = settings.EndWorkingHours,
            PhoneNumber = settings.PhoneNumber,
        };
    }
}

public class GetClubInfoModel
{
    public List<string> DaysOff { get; set; } = [];
    public string StartWorkingHours { get; set; } = string.Empty;
    public string EndWorkingHours { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}