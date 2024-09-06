namespace DH.Domain.Models.RoomModels.Queries;

public class GetRoomMessageListQueryModel
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
