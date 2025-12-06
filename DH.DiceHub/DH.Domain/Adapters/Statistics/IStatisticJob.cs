namespace DH.Domain.Adapters.Statistics;

public interface IStatisticJob
{
    string JobId { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public interface IStatisticJobInfo
{
    string JobId { get; }
    StatisticJobType JobType { get; }
}