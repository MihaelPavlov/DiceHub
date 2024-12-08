using DH.Domain.Enums;

namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetActiveBookedSpaceTableQueryModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int NumberOfGuests { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public ReservationStatus Status { get; set; }
    public string PublicNote { get; set; }
}
