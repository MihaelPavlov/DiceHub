namespace DH.Domain.Models.GameModels.Queries;

public class GetGameQrCodeListQueryModel
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string FileName { get; set; } = string.Empty;
}
