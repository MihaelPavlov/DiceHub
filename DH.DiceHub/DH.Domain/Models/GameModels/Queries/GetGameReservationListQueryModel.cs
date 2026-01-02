namespace DH.Domain.Models.GameModels.Queries;

public class GetGameReservationListQueryModel
{
    public int Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public int ReservedDurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public bool IsPaymentSuccessful { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameImageUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}