using DH.Domain.Entities;
using DH.Domain.Services.TenantSettingsService;
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

        var dto = new TenantSettingDto
        {
            Id = settings.Id,
            AverageMaxCapacity = settings.AverageMaxCapacity,
            ChallengeRewardsCountForPeriod = settings.ChallengeRewardsCountForPeriod,
            PeriodOfRewardReset = settings.PeriodOfRewardReset,
            ResetDayForRewards = settings.ResetDayForRewards,
            ChallengeInitiationDelayHours = settings.ChallengeInitiationDelayHours,
            ReservationHours = settings.ReservationHours.Split(", "),
            PhoneNumber = settings.PhoneNumber,
            BonusTimeAfterReservationExpiration = settings.BonusTimeAfterReservationExpiration,
        };

        return dto;
    }
}