using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateTenantSettingsCommand(TenantSettingDto Settings) : IRequest;

internal class UpdateTenantSettingsCommandHandler : IRequestHandler<UpdateTenantSettingsCommand>
{
    readonly IRepository<TenantSetting> repository;

    public UpdateTenantSettingsCommandHandler(IRepository<TenantSetting> repository)
    {
        this.repository = repository;
    }

    public async Task Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.Settings.Id == null)
        {
            await this.repository.AddAsync(new TenantSetting
            {
                ChallengeInitiationDelayHours = request.Settings.ChallengeInitiationDelayHours,
                ResetDayForRewards = request.Settings.ResetDayForRewards,
                AverageMaxCapacity = request.Settings.AverageMaxCapacity,
                ChallengeRewardsCountForPeriod = request.Settings.ChallengeRewardsCountForPeriod,
                PeriodOfRewardReset = request.Settings.PeriodOfRewardReset
            }, cancellationToken);

            return;
        }

        var dbSettings = await this.repository.GetByAsyncWithTracking(x => x.Id == request.Settings.Id, cancellationToken);

        if (dbSettings!.AverageMaxCapacity != request.Settings.AverageMaxCapacity)
        {
            dbSettings.AverageMaxCapacity = request.Settings.AverageMaxCapacity;
        }

        if (dbSettings.ChallengeRewardsCountForPeriod != request.Settings.ChallengeRewardsCountForPeriod)
        {
            dbSettings.ChallengeRewardsCountForPeriod = request.Settings.ChallengeRewardsCountForPeriod;
        }

        if (dbSettings.PeriodOfRewardReset != request.Settings.PeriodOfRewardReset)
        {
            dbSettings.PeriodOfRewardReset = request.Settings.PeriodOfRewardReset;
        }

        if (dbSettings.ResetDayForRewards != request.Settings.ResetDayForRewards)
        {
            dbSettings.ResetDayForRewards = request.Settings.ResetDayForRewards;
        }

        if (dbSettings.ChallengeInitiationDelayHours != request.Settings.ChallengeInitiationDelayHours)
        {
            dbSettings.ChallengeInitiationDelayHours = request.Settings.ChallengeInitiationDelayHours;
        }

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}