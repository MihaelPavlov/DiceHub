namespace DH.Domain.Entities;

public class ClubVisitorLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
