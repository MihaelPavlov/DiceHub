using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Helpers;
using DH.Domain.Repositories;
using DH.Domain.Services;

namespace DH.Adapter.Scheduling.Handlers;

/// <inheritdoc/>
internal class AddUserChallengePeriodHandler : IAddUserChallengePeriodHandler
{
    readonly IRepository<FailedJob> failedJobsRepository;
    readonly IRepository<UserChallengePeriodPerformance> userChallengePeriodPerformanceRepository;
    readonly IRepository<UserChallenge> userChallengesRepository;
    readonly IUserChallengesManagementService userChallengesManagementService;

    public AddUserChallengePeriodHandler(IRepository<FailedJob> failedJobsRepository, IUserChallengesManagementService userChallengesManagementService, IRepository<UserChallenge> userChallengesRepository, IRepository<UserChallengePeriodPerformance> userChallengePeriodPerformanceRepository)
    {
        this.failedJobsRepository = failedJobsRepository;
        this.userChallengesManagementService = userChallengesManagementService;
        this.userChallengesRepository = userChallengesRepository;
        this.userChallengePeriodPerformanceRepository = userChallengePeriodPerformanceRepository;
    }

    public async Task InitializeNewPeriod(CancellationToken cancellationToken)
    {
        var periods = await this.userChallengePeriodPerformanceRepository.GetWithPropertiesAsTrackingAsync(x => x.IsPeriodActive, x => x, cancellationToken);

        // for every active period
        foreach (var period in periods)
        {
            // Calculate the expected end date based on the StartDate and TimePeriodType
            DateTime expectedEndDate;
            switch (period.TimePeriodType)
            {
                case TimePeriodType.Weekly:
                    expectedEndDate = period.StartDate.AddDays(TimePeriodTypeHelper.GetDays(TimePeriodType.Weekly));
                    break;
                case TimePeriodType.Monthly:
                    int daysInMonth = DateTime.DaysInMonth(period.StartDate.Year, period.StartDate.Month);
                    expectedEndDate = period.StartDate.AddDays(daysInMonth);
                    break;
                case TimePeriodType.Yearly:
                    throw new NotImplementedException("Functionality for Yearly period is not implemented");
                default:
                    throw new InvalidOperationException("Unsupported TimePeriodType");
            }

            // Validate the EndDate
            if (period.EndDate.Date != expectedEndDate.Date)
                continue;

            var isInitiationSuccessfully = await this.userChallengesManagementService.InitiateUserChallengePeriod(period.UserId, cancellationToken);

            if (isInitiationSuccessfully)
            {
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