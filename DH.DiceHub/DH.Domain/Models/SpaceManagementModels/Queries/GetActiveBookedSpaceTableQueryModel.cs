namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetActiveBookedSpaceTableQueryModel
{
    public string Username { get; set; } = string.Empty;
    public int NumberOfGuests { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsConfirmed { get; set; }
}
