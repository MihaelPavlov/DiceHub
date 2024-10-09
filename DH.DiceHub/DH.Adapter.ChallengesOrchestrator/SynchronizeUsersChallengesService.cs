using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Data;
using DH.Domain.Helpers;
using DH.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.ChallengesOrchestrator;

internal class SynchronizeUsersChallengesService : BackgroundService, ISynchronizeUsersChallengesService
{
    readonly IUserChallengesManagementService userChallengesManagementService;
    readonly ILogger<SynchronizeUsersChallengesService> logger;
    readonly SynchronizeUsersChallengesQueue queue;

    public SynchronizeUsersChallengesService(ITenantDbContext tenantDbContext, ILogger<SynchronizeUsersChallengesService> logger, SynchronizeUsersChallengesQueue queue, IUserChallengesManagementService userChallengesManagementService)
    {
        this.userChallengesManagementService = userChallengesManagementService;
        this.logger = logger;
        this.queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (this.queue.TryDequeue(out var jobInfo))
            {
                try
                {
                    var jobStartTime = DateTime.UtcNow;

                    switch (jobInfo)
                    {
                        case SynchronizeUsersChallengesQueue.SynchronizeNewUserJob newUserJob:
                            LogStartInformation(jobInfo);
                            await SynchronizeNewUserJob(newUserJob, cancellationToken);
                            LogEndInformation(jobInfo, jobStartTime, DateTime.UtcNow);
                            break;
                        case SynchronizeUsersChallengesQueue.ChallengeInitiationJob challengeJob:
                            if (challengeJob.ScheduledTime.HasValue && DateTime.UtcNow >= challengeJob.ScheduledTime)
                            {
                                LogStartInformation(jobInfo);
                                await ChallengeInitiationJob(challengeJob, cancellationToken);
                                LogEndInformation(jobInfo, jobStartTime, DateTime.UtcNow);
                                break;
                            }
                            this.queue.AddChallengeInitiationJob(challengeJob.UserId, challengeJob.ScheduledTime.GetValueOrDefault());
                            this.logger.LogInformation("Job Requeue : {jobInfo}", JsonSerializer.Serialize(jobInfo));
                            break;

                        default:
                            this.logger.LogWarning("Unknown job type: {jobInfo}", JsonSerializer.Serialize(jobInfo));
                            break;
                    }
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping => ignore
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Job: {jobInfo}", JsonSerializer.Serialize(jobInfo));
                }
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public async Task SynchronizeNewUserJob(SynchronizeUsersChallengesQueue.SynchronizeNewUserJob job, CancellationToken cancellationToken)
    {
        await this.userChallengesManagementService.InitiateNewUserChallenges(job.UserId, cancellationToken);
    }

    public async Task ChallengeInitiationJob(SynchronizeUsersChallengesQueue.ChallengeInitiationJob job, CancellationToken cancellationToken)
    {
        await this.userChallengesManagementService.AddChallengeToUser(job.UserId, cancellationToken);
    }

    private void LogStartInformation(SynchronizeUsersChallengesQueue.JobInfo jobInfo)
    {
        logger.LogInformation("Job started: {jobInfo}", JsonSerializer.Serialize(jobInfo));
    }

    private void LogEndInformation(SynchronizeUsersChallengesQueue.JobInfo jobInfo, DateTime jobStartTime, DateTime jobEndTime)
    {
        logger.LogInformation("Job executed in {FormattedDuration}: {jobInfo}", DateFormattingHelper.GetFormatedDuration(jobStartTime, jobEndTime), JsonSerializer.Serialize(jobInfo));
    }
}
