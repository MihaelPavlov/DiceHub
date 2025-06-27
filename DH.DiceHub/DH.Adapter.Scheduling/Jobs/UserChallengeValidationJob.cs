using DH.Domain.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DH.Adapter.Scheduling.Jobs;

/// <summary>
/// A job responsible for handling the validation of the user challenge period. 
/// </summary>
[DisallowConcurrentExecution]
internal class UserChallengeValidationJob : IJob
{
    readonly IUserChallengesManagementService userChallengesManagementService;
    readonly ILogger<UserChallengeValidationJob> logger;

    public UserChallengeValidationJob(
        IUserChallengesManagementService userChallengesManagementService,
        ILogger<UserChallengeValidationJob> logger)
    {
        this.userChallengesManagementService = userChallengesManagementService;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await this.userChallengesManagementService.EnsureValidUserChallengePeriodsAsync(context.CancellationToken);
            this.logger.LogInformation("User Challenge Period Validation check completed.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed during User Challenge Period Validation.");
        }
    }
}
