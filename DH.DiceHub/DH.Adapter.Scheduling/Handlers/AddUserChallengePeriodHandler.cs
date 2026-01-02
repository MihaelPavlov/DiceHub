using DH.Domain.Adapters.Scheduling;
using DH.Domain.Adapters.Scheduling.Enums;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.Domain.Services;

namespace DH.Adapter.Scheduling.Handlers;

/// <summary>
/// Implementation of <see cref="IAddUserChallengePeriodHandler"/>
/// </summary>
internal class AddUserChallengePeriodHandler : IAddUserChallengePeriodHandler
{
    readonly IRepository<FailedJob> failedJobsRepository;
    readonly IUserChallengesManagementService userChallengesManagementService;

    public AddUserChallengePeriodHandler(
        IRepository<FailedJob> failedJobsRepository,
        IUserChallengesManagementService userChallengesManagementService)
    {
        this.failedJobsRepository = failedJobsRepository;
        this.userChallengesManagementService = userChallengesManagementService;
    }

    public async Task InitializeNewPeriods(CancellationToken cancellationToken)
    {
        await this.userChallengesManagementService.InitializeNewPeriodsBatch(cancellationToken);
    }

    public async Task ProcessFailedReset(string data, string errorMessage, CancellationToken cancellationToken)
    {
        await failedJobsRepository.AddAsync(new FailedJob { Data = data, Type = (int)JobType.AddUserChallengePeriod, FailedAt = DateTime.UtcNow, ErrorMessage = errorMessage }, cancellationToken);
    }
}