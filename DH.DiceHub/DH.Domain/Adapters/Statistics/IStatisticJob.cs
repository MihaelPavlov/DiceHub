namespace DH.Domain.Adapters.Statistics;

public interface IStatisticJob
{
    Guid JobId { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public interface IStatisticJobInfo
{
    Guid JobId { get; }
}