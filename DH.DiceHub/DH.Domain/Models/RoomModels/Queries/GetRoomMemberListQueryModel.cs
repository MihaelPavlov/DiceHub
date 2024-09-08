namespace DH.Domain.Models.RoomModels.Queries;

public class GetRoomMemberListQueryModel
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime JoinedAt { get; set; }
}
