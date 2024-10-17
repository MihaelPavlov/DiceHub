namespace DH.Domain.Adapters.Scheduling;

/// <summary>
/// Interface responsible for evaluating the expiration status of user rewards
/// and handling failed expiration checks.
/// </summary>
public interface IUserRewardsExpiryHandler
{
    /// <summary>
    /// Evaluates whether any user rewards have expired, marking them as expired
    /// if their expiration date has passed, and saves changes to the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EvaluateUserRewardsExpiry(CancellationToken cancellationToken);

    /// <summary>
    /// Processes failed attempts at evaluating the expiration status of rewards.
    /// This method logs the failed job details, such as data and error messages,
    /// to a repository for future analysis or reprocessing.
    /// </summary>
    /// <param name="data">The data related to the failed job.</param>
    /// <param name="errorMessage">The error message encountered during the failed process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessFailedExpiryCheck(string data, string errorMessage, CancellationToken cancellationToken);
}
