using DH.Domain.Enums;

namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetSpaceTableReservationByIdQueryModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int NumberOfGuests { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsReservationSuccessful { get; set; }
    public ReservationStatus Status { get; set; }
    public string InternalNote { get; set; } = string.Empty;
    public string PublicNote { get; set; } = string.Empty;
}
