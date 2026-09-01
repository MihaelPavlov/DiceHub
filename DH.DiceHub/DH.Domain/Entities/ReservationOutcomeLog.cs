using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class ReservationOutcomeLog : TenantEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the reservation.
    /// </summary>
    public int ReservationId { get; set; }

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the reservation outcome occurred (either completed or cancelled).
    /// </summary>
    public DateTime OutcomeDate { get; set; }

    /// <summary>
    /// Date of the log creation
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the outcome action (Completed or Cancelled).
    /// </summary>
    public ReservationOutcome Outcome { get; set; } 
    
    /// <summary>
    /// Gets or sets the type of the reservation (Game or Table).
    /// </summary>
    public ReservationType Type { get; set; }
}
