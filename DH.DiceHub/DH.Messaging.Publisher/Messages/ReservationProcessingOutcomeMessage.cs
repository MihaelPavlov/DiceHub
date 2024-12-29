namespace DH.Messaging.Publisher.Messages;

public class ReservationProcessingOutcomeMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the reservation.
    /// </summary>
    public int ReservationId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the reservation outcome occurred (either completed or cancelled).
    /// </summary>
    public DateTime OutcomeDate { get; set; }

    /// <summary>
    /// Gets or sets the outcome action (Completed or Cancelled).
    /// </summary>
    public ReservationOutcome Outcome { get; set; }

    /// <summary>
    /// Gets or sets the type of the reservation (Game or Table).
    /// </summary>
    public ReservationType Type { get; set; }
}

public enum ReservationOutcome
{
    Completed = 0,
    Cancelled = 1
}

public enum ReservationType
{
    Game = 0,
    Table = 1
}