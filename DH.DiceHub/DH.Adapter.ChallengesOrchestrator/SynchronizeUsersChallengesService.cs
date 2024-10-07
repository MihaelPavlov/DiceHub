using DH.Domain.Adapters.ChallengesOrchestrator;
using DH.Domain.Adapters.Data;
using DH.Domain.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DH.Adapter.ChallengesOrchestrator;

internal class SynchronizeUsersChallengesService : BackgroundService, ISynchronizeUsersChallengesService
{
    readonly ITenantDbContext _tenantDbContext;
    readonly ILogger<SynchronizeUsersChallengesService> _logger;
    readonly SynchronizeUsersChallengesQueue _queue;

    public SynchronizeUsersChallengesService(ITenantDbContext tenantDbContext, ILogger<SynchronizeUsersChallengesService> logger, SynchronizeUsersChallengesQueue queue)
    {
        _tenantDbContext = tenantDbContext;
        _logger = logger;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var jobInfo))
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
                            _queue.AddChallengeInitiationJob(challengeJob.UserId, challengeJob.ScheduledTime.GetValueOrDefault());
                            _logger.LogInformation("Job Requeue : {jobInfo}", JsonSerializer.Serialize(jobInfo));
                            break;

                        default:
                            _logger.LogWarning("Unknown job type: {jobInfo}", JsonSerializer.Serialize(jobInfo));
                            break;
                    }
                }
                catch (TaskCanceledException)
                {
                    // Application is stopping => ignore
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job: {jobInfo}", JsonSerializer.Serialize(jobInfo));
                }
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public Task SynchronizeNewUserJob(SynchronizeUsersChallengesQueue.SynchronizeNewUserJob job, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ChallengeInitiationJob(SynchronizeUsersChallengesQueue.ChallengeInitiationJob job, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void LogStartInformation(SynchronizeUsersChallengesQueue.JobInfo jobInfo)
    {
        _logger.LogInformation("Job started: {jobInfo}", JsonSerializer.Serialize(jobInfo));
    }

    private void LogEndInformation(SynchronizeUsersChallengesQueue.JobInfo jobInfo, DateTime jobStartTime, DateTime jobEndTime)
    {
        _logger.LogInformation("Job executed in {FormattedDuration}: {jobInfo}", DateFormattingHelper.GetFormatedDuration(jobStartTime, jobEndTime), JsonSerializer.Serialize(jobInfo));
    }
}
