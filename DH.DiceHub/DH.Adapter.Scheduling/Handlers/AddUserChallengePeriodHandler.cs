using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IAddUserChallengePeriodHandler"/>
/// </summary>
internal class AddUserChallengePeriodHandler : IAddUserChallengePeriodHandler
{
    readonly IRepository<FailedJob> failedJobsRepository;
    readonly IRepository<UserChallengePeriodPerformance> userChallengePeriodPerformanceRepository;
    readonly IUserChallengesManagementService userChallengesManagementService;

    public AddUserChallengePeriodHandler(
        IRepository<FailedJob> failedJobsRepository,
        IUserChallengesManagementService userChallengesManagementService,
        IRepository<UserChallengePeriodPerformance> userChallengePeriodPerformanceRepository)
    {
        this.failedJobsRepository = failedJobsRepository;
        this.userChallengesManagementService = userChallengesManagementService;
        this.userChallengePeriodPerformanceRepository = userChallengePeriodPerformanceRepository;
    }

    public async Task InitializeNewPeriods(CancellationToken cancellationToken)
    {
        var periods = await this.userChallengePeriodPerformanceRepository.GetWithPropertiesAsTrackingAsync(x => x.IsPeriodActive, x => x, cancellationToken);

        // for every active period
        foreach (var period in periods)
        {
            var isInitiationSuccessfully = await this.userChallengesManagementService.InitiateUserChallengePeriod(period.UserId, cancellationToken);

            if (isInitiationSuccessfully)
            {
                //TODO: After disabling the old performance period do we make Inactive the old challenges??
                period.IsPeriodActive = false;
            }
        }
        await this.userChallengePeriodPerformanceRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessFailedReset(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.AddUserChallengePeriod, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }
}