using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateTenantSettingsCommand(TenantSettingDto Settings) : IRequest;

internal class UpdateTenantSettingsCommandHandler(IRepository<TenantSetting> repository) : IRequestHandler<UpdateTenantSettingsCommand>
{
    readonly IRepository<TenantSetting> repository = repository;

    public async Task Handle(UpdateTenantSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.Settings.FieldsAreValid(out var validationErrors))
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
                ReservationHours = string.Join(",", request.Settings.ReservationHours.OrderBy(x => x)),
                BonusTimeAfterReservationExpiration = request.Settings.BonusTimeAfterReservationExpiration,
                PhoneNumber = request.Settings.PhoneNumber,
                ClubName = request.Settings.ClubName,
                IsCustomPeriodOn = request.Settings.IsCustomPeriodOn,
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
        }

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}