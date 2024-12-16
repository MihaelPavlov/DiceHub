using DH.Domain.Entities;
using DH.Domain.Exceptions;
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

        if (dbSettings.BonusTimeAfterReservationExpiration != request.Settings.BonusTimeAfterReservationExpiration)
        {
            dbSettings.BonusTimeAfterReservationExpiration = request.Settings.BonusTimeAfterReservationExpiration;
        }


        await this.repository.SaveChangesAsync(cancellationToken);
    }
}