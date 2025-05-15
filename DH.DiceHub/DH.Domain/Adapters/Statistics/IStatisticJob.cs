namespace DH.Domain.Adapters.Statistics;

public interface IStatisticJob
{
    Guid JobId { get; }
    Task<bool> ExecuteAsync(CancellationToken cancellationToken);
}

public interface IStatisticJobInfo
{
    Guid JobId { get; }
    StatisticJobType JobType { get; }
}