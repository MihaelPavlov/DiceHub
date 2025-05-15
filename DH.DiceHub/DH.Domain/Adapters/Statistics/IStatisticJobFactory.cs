namespace DH.Domain.Adapters.Statistics;

public interface IStatisticJobFactory
{
    IStatisticJob CreateHandler(IStatisticJobInfo jobInfo);
}
