namespace DH.Domain.Services;

/// <summary>
/// This interface defines the contract for services that manage game session operations,
/// specifically focusing on processing challenges associated with user gameplay.
/// It facilitates the interaction between the game sessions and the challenge system,
/// ensuring that users meet the requirements necessary to complete challenges after gameplay.
/// </summary>
public interface IGameSessionService
{
    /// <summary>
    /// Processes the challenges associated with a user's game session after it has concluded.
    /// This method evaluates whether the user has met the necessary requirements to complete the challenges
    /// based on their gameplay and the game played.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose game session has ended. 
    /// This parameter is used to track and validate the user's progress and challenge completion status.</param>
    /// <param name="gameId">The unique identifier of the game that was played. 
    /// This parameter helps determine which challenges are applicable for processing based on the completed game.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task</bool>"/> representing the asynchronous operation. Represent is the processing was successfully.
    /// The task will complete when the challenge processing is finished, allowing for non-blocking execution.</returns>
    Task<bool> ProcessChallengeAfterSession(string userId, int gameId, CancellationToken cancellationToken);

    /// <summary>
    /// Collects reward points that the user has earned by completing challenges. 
    /// This method ensures that all reward points, based on the user's completed challenges, 
    /// are properly accumulated and made available for the user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom reward points are being collected. 
    /// This parameter helps track the user's challenge completions and determine the applicable reward points.</param>
    /// <param name="cancellationToken">A token used to cancel the operation if needed. 
    /// This allows for cooperative cancellation of the task, particularly in long-running operations.</param>
    /// <returns>A <see cref="Task</bool>"/> representing the asynchronous operation. 
    /// The task will complete once the reward points collection process is finished, indicating whether the operation was successful.</returns>
    Task<bool> CollectRewardsFromChallenges(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Evaluates the user's rewards based on the completion of challenges after the game session.
    /// This method checks if any rewards can now be claimed by the user as a result of successfully completing their challenges.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the rewards are being evaluated. 
    /// This parameter is essential to track the user's reward status and determine eligible rewards.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.
    /// The task will complete once the evaluation of rewards is finished, allowing for non-blocking execution.</returns>
    Task EvaluateUserRewards(string userId, CancellationToken cancellationToken);
}
