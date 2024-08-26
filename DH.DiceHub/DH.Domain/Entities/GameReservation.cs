namespace DH.Domain.Entities;

public class GameReservation
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public int ReservedDurationMinutes { get; set; }
    public bool IsPaymentSuccessful { get; set; }
    public bool IsExpired { get; set; } = false;
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
