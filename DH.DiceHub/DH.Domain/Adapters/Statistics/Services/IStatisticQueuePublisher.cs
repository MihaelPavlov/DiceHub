namespace DH.Domain.Adapters.Statistics.Services;

// Check all places for *PublishEvent*
public interface IStatisticQueuePublisher
{
    Task PublishAsync<TJob>(TJob job) where TJob : IStatisticJobInfo;
}