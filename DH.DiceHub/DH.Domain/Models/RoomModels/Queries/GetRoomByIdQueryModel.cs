namespace DH.Domain.Models.RoomModels.Queries;
public class GetRoomByIdQueryModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int JoinedParticipants { get; set; }
    public DateTime StartDate { get; set; }
    public int MaxParticipants { get; set; }
    public int GameId { get; set; }
    public int GameImageId { get; set; }
    public string Username { get; set; } = string.Empty;
}