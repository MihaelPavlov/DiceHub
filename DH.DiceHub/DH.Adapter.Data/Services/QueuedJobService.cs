using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Services.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DH.Adapter.Data.Services;

public class QueuedJobService : IQueuedJobService
{
    readonly IDbContextFactory<TenantDbContext> contextFactory;
    readonly ILogger<QueuedJobService> logger;

    public QueuedJobService(IDbContextFactory<TenantDbContext> contextFactory, ILogger<QueuedJobService> logger)
    {
        this.logger = logger;
        this.contextFactory = contextFactory;
    }

    public async Task Create(string queueName,Guid jobId, string payload)
    {
        using (var context = await this.contextFactory.CreateDbContextAsync())
        {
            await context.QueuedJobs.AddAsync(new QueuedJob
            {
                JobId = jobId,
                MessagePayload = payload,
                Status = JobStatus.Pending,
                QueueType = queueName,
                EnqueuedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
    }

    public async Task UpdatePayload(string queueName, Guid jobId, string payload)
    {
        using (var context = await this.contextFactory.CreateDbContextAsync())
        {
            var job = await context.QueuedJobs
                .FirstOrDefaultAsync(x => x.QueueType == queueName && x.JobId == jobId);

            if (job is null)
            {
                logger.LogError("During UpdatePayload: Job not found: {queueName} - {jobId}", queueName, jobId);
                return;
            }

            job.MessagePayload = payload;
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusToCompleted(string queueName, Guid jobId)
    {
        var isUpdatedSuccesuflly = await this.UpdateJobStatus(queueName, jobId, JobStatus.Completed);

        if (!isUpdatedSuccesuflly)
        {
            logger.LogError("During UpdateStatusToCompleted Job not found: {queueName} - {jobId}", queueName, jobId);
        }
    }

    public async Task UpdateStatusToFailed(string queueName, Guid jobId)
    {
        var isUpdatedSuccesuflly = await this.UpdateJobStatus(queueName, jobId, JobStatus.Failed);

        if (!isUpdatedSuccesuflly)
        {
            logger.LogError("During UpdateStatusToFailed: Job not found: {queueName} - {jobId}", queueName, jobId);
        }
    }

    private async Task<bool> UpdateJobStatus(string queueName, Guid jobId, JobStatus jobStatus)
    {
        using (var context = await this.contextFactory.CreateDbContextAsync())
        {
            var job = await context.QueuedJobs
                .FirstOrDefaultAsync(x => x.QueueType == queueName && x.JobId == jobId);
            if (job != null)
            {
                job.Status = jobStatus;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
