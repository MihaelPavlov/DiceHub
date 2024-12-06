using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class GameReservation
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public int ReservedDurationMinutes { get; set; }
    public int PeopleCount { get; set; }
    public bool IsPaymentSuccessful { get; set; }

    /// <summary>
    /// Indicates if the reservation is still active and it's waithing for aprrove or decline
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if the reservation is approved, declined or without status from the staff
    /// </summary>
    public ReservationStatus Status { get; set; }

    public bool IsReservationSuccessful { get; set; }
    public int GameId { get; set; }
    public string InternalNote { get; set; } = string.Empty;
    public string PublicNote { get; set; } = string.Empty;

    public virtual Game Game { get; set; } = null!;
}
