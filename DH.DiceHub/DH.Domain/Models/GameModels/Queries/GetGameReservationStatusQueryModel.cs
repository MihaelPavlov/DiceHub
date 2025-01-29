using DH.Domain.Enums;

namespace DH.Domain.Models.GameModels.Queries;

public class GetGameReservationStatusQueryModel
{
    public int ReservationId { get; set; }
    public int GameId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int ReservedDurationMinutes { get; set; }
    public bool IsActive { get; set; } = false;
    public ReservationStatus Status { get; set; }
    public string PublicNote { get; set; } = string.Empty;
}
