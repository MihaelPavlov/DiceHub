namespace DH.Messaging.Publisher.Messages;

public class ChallengeProcessingOutcomeMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the challenge.
    /// </summary>
    public int ChallengeId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the challenge outcome occurred
    /// </summary>
    public DateTime OutcomeDate { get; set; }

    /// <summary>
    /// Gets or sets the outcome action (Completed or Expired).
    /// </summary>
    public ChallengeOutcome Outcome { get; set; }
}

public enum ChallengeOutcome
{
    Completed = 0,
    Expired = 1
}