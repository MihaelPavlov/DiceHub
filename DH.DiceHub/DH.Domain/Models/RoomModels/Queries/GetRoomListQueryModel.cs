namespace DH.Domain.Models.RoomModels.Queries;

public class GetRoomListQueryModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int JoinedParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public int MaxParticipants { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameImageUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
