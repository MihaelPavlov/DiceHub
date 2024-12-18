namespace DH.Domain.Adapters.Scheduling;

/// <summary>
/// Interface responsible for handling user reward expiration reminders, 
/// including evaluating upcoming reward expirations and processing failed reminder jobs.
/// </summary>
public interface IUserRewardsExpirationReminderHandler
{
    /// <summary>
    /// Evaluates rewards nearing expiration and sends reminders to users. 
    /// This method checks for rewards that are about to expire within a defined time frame 
    /// (e.g., 3, 2, 1 days) and triggers notifications to the respective users.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of evaluating rewards and sending reminders.</returns>
    Task EvaluateRewardsReminder(CancellationToken cancellationToken);

    /// <summary>
    /// Processes failed attempts at sending expiration reminders for rewards. 
    /// This method logs the details of the failed job, such as the data and error message, 
    /// to a repository for analysis or reprocessing at a later time.
    /// </summary>
    /// <param name="data">The data related to the failed job.</param>
    /// <param name="errorMessage">The error message encountered during the failed process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessFailedRewardExpirationReminder(string data, string errorMessage, CancellationToken cancellationToken);
}
