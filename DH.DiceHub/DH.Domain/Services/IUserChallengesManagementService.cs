namespace DH.Domain.Services;

/// <summary>
/// Interface for managing user challenges in the system. 
/// Provides methods to initialize challenges for new users and to add additional challenges based on their progress.
/// </summary>
public interface IUserChallengesManagementService
{
    Task InitializeNewPeriodsBatch(CancellationToken cancellationToken);
    /// <summary>
    /// Initiate challenge period for user. 
    /// The system generates rewards and assigns initial challenges.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to initialize challenges.</param>
    /// <param name="forNewUser">Is the user new or no.</param>
    /// <param name="cancellationToken">Cancellation token for the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<bool> InitiateUserChallengePeriod(string userId, CancellationToken cancellationToken, bool forNewUser = false);

    /// <summary>
    /// Adds a new challenge to the user. 
    /// Depending on the user's current active challenges, either an in-progress or locked challenge is assigned. 
    /// Required points for locked challenges are calculated based on the user's previous challenges.
    /// </summary>
    /// <param name="userId">The ID of the user to assign a new challenge to.</param>
    /// <param name="cancellationToken">Cancellation token for the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AssignNextChallengeToUserAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Ensures that all users have a valid and active <see cref="UserChallengePeriodPerformance"/>.
    /// <para>
    /// This method performs validation for each user in the system. If a user's current challenge period is missing
    /// or falls outside the valid date range, the method deactivates the existing period (if present)
    /// and initializes a new valid challenge period based on global tenant settings.
    /// </para>
    /// <para>
    /// Each update is wrapped in its own database transaction per user to ensure data consistency and allow partial progress if errors occur.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureValidUserChallengePeriodsAsync(CancellationToken cancellationToken);
}
