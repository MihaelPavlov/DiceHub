namespace DH.Domain.Adapters.Statistics.Services;

public interface IStatisticQueuePublisher
{
    Task PublishAsync<TJob>(TJob job) where TJob : IStatisticJobInfo;
}