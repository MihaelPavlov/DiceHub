using DH.Domain.Enums;

namespace DH.Domain.Models.GameModels.Queries;

public class GetActiveGameReservationListQueryModel
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameImageUrl { get; set; } = string.Empty;
    public int NumberOfGuests { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ReservationDate { get; set; }
    public int ReservedDurationMinutes { get; set; }
    public ReservationStatus Status { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserLanguage { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool UserHaveActiveTableReservation { get; set; }
    public DateTime? TableReservationTime { get; set; }
}
