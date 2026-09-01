using DH.Domain.Enums;

namespace DH.Domain.Entities;

public class SpaceTableReservation : TenantEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int NumberOfGuests { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Indicates if the reservation is still active and it's waiting for approve or decline
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if the reservation is approved, declined or without status from the staff
    /// </summary>
    public ReservationStatus Status { get; set; }
    public string InternalNote { get; set; } = string.Empty;
    public string PublicNote { get; set; } = string.Empty;

    public bool IsReservationSuccessful { get; set; }
}
