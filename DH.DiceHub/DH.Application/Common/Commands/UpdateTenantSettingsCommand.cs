using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Common.Commands;

public record UpdateTenantSettingsCommand(TenantSettingDto Settings) : IRequest;

internal class UpdateTenantSettingsCommandHandler(
    IRepository<TenantSetting> repository,
    IRepository<CustomPeriodChallenge> customPeridoChallengesRepository,
    IRepository<CustomPeriodReward> customPeridoRewardsRepository,
    ILocalizationService localizer,
    IUserContext userContext,
    ILogger<UpdateTenantSettingsCommandHandler> logger,
    ISchedulerService schedulerService) : IRequestHandler<UpdateTenantSettingsCommand>
{
    readonly IRepository<TenantSetting> repository = repository;
    readonly IRepository<CustomPeriodChallenge> customPeridoChallengesRepository = customPeridoChallengesRepository;
    readonly IRepository<CustomPeriodReward> customPeridoRewardsRepository = customPeridoRewardsRepository;
    readonly ILocalizationService localizer = localizer;
    readonly IUserContext userContext = userContext;
    readonly ILogger<UpdateTenantSettingsCommandHandler> logger = logger;
    readonly ISchedulerService schedulerService = schedulerService;

    public async Task Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Settings.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        if (request.Settings.Id == null)
        {
            await this.repository.AddAsync(new TenantSetting
            {
                ChallengeInitiationDelayHours = request.Settings.ChallengeInitiationDelayHours,
                ResetDayForRewards = request.Settings.ResetDayForRewards,
                AverageMaxCapacity = request.Settings.AverageMaxCapacity,
                ChallengeRewardsCountForPeriod = request.Settings.ChallengeRewardsCountForPeriod,
                PeriodOfRewardReset = request.Settings.PeriodOfRewardReset,
                StartWorkingHours = request.Settings.StartWorkingHours,
                EndWorkingHours = request.Settings.EndWorkingHours,
                TimeZoneId = request.Settings.TimeZoneId,
                DaysOff = string.Join(",", request.Settings.DaysOff.OrderBy(x => x)),
                ReservationHours = string.Join(",", request.Settings.ReservationHours.OrderBy(x => x)),
                BonusTimeAfterReservationExpiration = request.Settings.BonusTimeAfterReservationExpiration,
                PhoneNumber = request.Settings.PhoneNumber,
                ClubName = request.Settings.ClubName,
                IsCustomPeriodOn = request.Settings.IsCustomPeriodOn,
            }, cancellationToken);

            await ScheduleTenantDailyJobs(request.Settings.EndWorkingHours, request.Settings.TimeZoneId, cancellationToken);
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

        if (dbSettings.StartWorkingHours != request.Settings.StartWorkingHours)
        {
            dbSettings.StartWorkingHours = request.Settings.StartWorkingHours;
        }
        if (dbSettings.EndWorkingHours != request.Settings.EndWorkingHours)
        {
            dbSettings.EndWorkingHours = request.Settings.EndWorkingHours;
        }

        if (dbSettings.TimeZoneId != request.Settings.TimeZoneId)
        {
            dbSettings.TimeZoneId = request.Settings.TimeZoneId;
        }

        if (dbSettings.DaysOff != string.Join(",", request.Settings.DaysOff.OrderBy(x => x)))
        {
            dbSettings.DaysOff = string.Join(",", request.Settings.DaysOff.OrderBy(x => x));
        }

        if (dbSettings.ChallengeInitiationDelayHours != request.Settings.ChallengeInitiationDelayHours)
        {
            dbSettings.ChallengeInitiationDelayHours = request.Settings.ChallengeInitiationDelayHours;
        }

        if (dbSettings.ReservationHours != string.Join(",", request.Settings.ReservationHours.OrderBy(x => x)))
        {
            dbSettings.ReservationHours = string.Join(",", request.Settings.ReservationHours.OrderBy(x => x));
        }

        if (dbSettings.PhoneNumber != request.Settings.PhoneNumber)
        {
            dbSettings.PhoneNumber = request.Settings.PhoneNumber;
        }

        if (dbSettings.ClubName != request.Settings.ClubName)
        {
            dbSettings.ClubName = request.Settings.ClubName;
        }

        if (dbSettings.BonusTimeAfterReservationExpiration != request.Settings.BonusTimeAfterReservationExpiration)
        {
            dbSettings.BonusTimeAfterReservationExpiration = request.Settings.BonusTimeAfterReservationExpiration;
        }

        if (dbSettings.IsCustomPeriodOn != request.Settings.IsCustomPeriodOn)
        {
            dbSettings.IsCustomPeriodOn = request.Settings.IsCustomPeriodOn;

            if (dbSettings.IsCustomPeriodOn)
            {
                var rewards = await this.customPeridoRewardsRepository.GetWithPropertiesAsync(x => x.Id != 0, x => x.Id, cancellationToken);
                var challenges = await this.customPeridoChallengesRepository.GetWithPropertiesAsync(x => x.Id != 0, x => x.Id, cancellationToken);

                if (rewards.Count != 0 && challenges.Count != 0)
                    dbSettings.IsCustomPeriodSetupComplete = true;
                else
                    dbSettings.IsCustomPeriodSetupComplete = false;
            }
        }

        await this.repository.SaveChangesAsync(cancellationToken);

        // Always refresh the per-tenant daily job triggers on save, not only when
        // working hours changed: a tenant whose triggers were never created
        // (predating these jobs, or lost with the Quartz store) would otherwise
        // never get them just by re-saving settings. Also picks up a time-zone
        // change - the tenant's own TimeZoneId, not the editing admin's browser
        // zone, decides when each fires.
        await ScheduleTenantDailyJobs(request.Settings.EndWorkingHours, request.Settings.TimeZoneId, cancellationToken);
    }

    async Task ScheduleTenantDailyJobs(string endWorkingHours, string timeZoneId, CancellationToken cancellationToken)
    {
        var tenantId = this.userContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            this.logger.LogWarning("Skipped scheduling per-tenant daily jobs after settings update: no ambient tenant context.");
            return;
        }

        try
        {
            await this.schedulerService.ScheduleTenantDailyJobsAsync(
                tenantId, endWorkingHours, timeZoneId, cancellationToken);

            // Reset cadence / day / time zone can all have changed here - re-arm the
            // per-tenant challenge-period trigger for the new next-reset date.
            await this.schedulerService.ScheduleAddUserPeriodJobForTenant(
                tenantId, replaceExisting: true, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex,
                "Failed to schedule per-tenant daily jobs for tenant {TenantId} after settings update.", tenantId);
        }
    }
}