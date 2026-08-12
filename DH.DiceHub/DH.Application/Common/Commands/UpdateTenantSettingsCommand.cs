using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateTenantSettingsCommand(TenantSettingDto Settings) : IRequest;

internal class UpdateTenantSettingsCommandHandler(
    IRepository<TenantSetting> repository,
    ITenantService tenantService,
    IRepository<CustomPeriodChallenge> customPeridoChallengesRepository,
    IRepository<CustomPeriodReward> customPeridoRewardsRepository,
    ILocalizationService localizer,
    ISchedulerService schedulerService,
    ITenantSettingsCacheService tenantSettingsCacheService) : IRequestHandler<UpdateTenantSettingsCommand>
{
    readonly IRepository<TenantSetting> repository = repository;
    readonly ITenantService tenantService = tenantService;
    readonly IRepository<CustomPeriodChallenge> customPeridoChallengesRepository = customPeridoChallengesRepository;
    readonly IRepository<CustomPeriodReward> customPeridoRewardsRepository = customPeridoRewardsRepository;
    readonly ILocalizationService localizer = localizer;
    readonly ISchedulerService schedulerService = schedulerService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;

    public async Task Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Settings.FieldsAreValid(out var validationErrors, this.localizer))
            throw new ValidationErrorsException(validationErrors);

        var tenant = await this.tenantService.GetCurrentTenantAsync(cancellationToken);
        var dbSettings = tenant.TenantSettingId != 0
            ? await this.repository.GetByAsyncWithTracking(x => x.Id == tenant.TenantSettingId, cancellationToken)
            : null;

        if (dbSettings == null)
        {
            dbSettings = await this.repository.AddAsync(new TenantSetting(), cancellationToken);
            tenant.TenantSettingId = dbSettings.Id;
        }

        if (dbSettings.AverageMaxCapacity != request.Settings.AverageMaxCapacity)
            dbSettings.AverageMaxCapacity = request.Settings.AverageMaxCapacity;

        if (dbSettings.ChallengeRewardsCountForPeriod != request.Settings.ChallengeRewardsCountForPeriod)
            dbSettings.ChallengeRewardsCountForPeriod = request.Settings.ChallengeRewardsCountForPeriod;

        if (dbSettings.PeriodOfRewardReset != request.Settings.PeriodOfRewardReset)
            dbSettings.PeriodOfRewardReset = request.Settings.PeriodOfRewardReset;

        if (dbSettings.ResetDayForRewards != request.Settings.ResetDayForRewards)
            dbSettings.ResetDayForRewards = request.Settings.ResetDayForRewards;

        if (dbSettings.StartWorkingHours != request.Settings.StartWorkingHours)
            dbSettings.StartWorkingHours = request.Settings.StartWorkingHours;

        var wasEndWorkingHourUpdated = false;
        if (dbSettings.EndWorkingHours != request.Settings.EndWorkingHours)
        {
            wasEndWorkingHourUpdated = true;
            dbSettings.EndWorkingHours = request.Settings.EndWorkingHours;
        }

        var daysOff = string.Join(",", request.Settings.DaysOff.OrderBy(x => x));
        if (dbSettings.DaysOff != daysOff)
            dbSettings.DaysOff = daysOff;

        if (dbSettings.ChallengeInitiationDelayHours != request.Settings.ChallengeInitiationDelayHours)
            dbSettings.ChallengeInitiationDelayHours = request.Settings.ChallengeInitiationDelayHours;

        var reservationHours = string.Join(",", request.Settings.ReservationHours.OrderBy(x => x));
        if (dbSettings.ReservationHours != reservationHours)
            dbSettings.ReservationHours = reservationHours;

        if (dbSettings.PhoneNumber != request.Settings.PhoneNumber)
            dbSettings.PhoneNumber = request.Settings.PhoneNumber;

        if (dbSettings.ClubName != request.Settings.ClubName)
            dbSettings.ClubName = request.Settings.ClubName;

        if (dbSettings.BonusTimeAfterReservationExpiration != request.Settings.BonusTimeAfterReservationExpiration)
            dbSettings.BonusTimeAfterReservationExpiration = request.Settings.BonusTimeAfterReservationExpiration;

        if (dbSettings.IsCustomPeriodOn != request.Settings.IsCustomPeriodOn)
        {
            dbSettings.IsCustomPeriodOn = request.Settings.IsCustomPeriodOn;

            if (dbSettings.IsCustomPeriodOn)
            {
                var rewards = await this.customPeridoRewardsRepository.GetWithPropertiesAsync(x => x.Id != 0, x => x.Id, cancellationToken);
                var challenges = await this.customPeridoChallengesRepository.GetWithPropertiesAsync(x => x.Id != 0, x => x.Id, cancellationToken);
                dbSettings.IsCustomPeriodSetupComplete = rewards.Count != 0 && challenges.Count != 0;
            }
        }

        await this.repository.SaveChangesAsync(cancellationToken);
        this.tenantSettingsCacheService.Clear(tenant.Id);

        if (wasEndWorkingHourUpdated)
            await this.schedulerService.ScheduleCloseActiveTablesJob(cancellationToken);
    }
}
