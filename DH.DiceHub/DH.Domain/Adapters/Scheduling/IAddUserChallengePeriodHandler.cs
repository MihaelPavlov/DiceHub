namespace DH.Domain.Adapters.Scheduling;

public interface IAddUserChallengePeriodHandler
{
    Task InitializeNewPeriods(CancellationToken cancellationToken);

    /// <summary>
    /// Same as <see cref="InitializeNewPeriods(CancellationToken)"/> but scoped to a single tenant.
    /// </summary>
    Task InitializeNewPeriods(string tenantId, CancellationToken cancellationToken);

    Task ProcessFailedReset(string data, string errorMessage, CancellationToken cancellationToken);
}
