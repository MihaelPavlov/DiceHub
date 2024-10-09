namespace DH.Domain.Services;

/// <summary>
/// Interface for managing user challenges in the system. 
/// Provides methods to initialize challenges for new users and to add additional challenges based on their progress.
/// </summary>
public interface IUserChallengesManagementService
{
    /// <summary>
    /// Initializes new challenges for a user. 
    /// If the user has no challenges, the system generates and assigns initial challenges.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to initialize challenges.</param>
    /// <param name="cancellationToken">Cancellation token for the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitiateNewUserChallenges(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new challenge to the user. 
    /// Depending on the user's current active challenges, either an in-progress or locked challenge is assigned. 
    /// Required points for locked challenges are calculated based on the user's previous challenges.
    /// </summary>
    /// <param name="userId">The ID of the user to assign a new challenge to.</param>
    /// <param name="cancellationToken">Cancellation token for the task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddChallengeToUser(string userId, CancellationToken cancellationToken);
}
