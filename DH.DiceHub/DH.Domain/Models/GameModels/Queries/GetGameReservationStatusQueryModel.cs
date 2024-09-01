namespace DH.Domain.Models.GameModels.Queries;

public class GetGameReservationStatusQueryModel
{
    public int GameId { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public int ReservedDurationMinutes { get; set; }
    public bool IsExpired { get; set; } = false;
}
