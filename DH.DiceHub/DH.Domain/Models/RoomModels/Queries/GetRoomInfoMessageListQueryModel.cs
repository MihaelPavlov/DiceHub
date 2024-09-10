namespace DH.Domain.Models.RoomModels.Queries;

public class GetRoomInfoMessageListQueryModel
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
