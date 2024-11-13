namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetSpaceTableByIdQueryModel
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public string Password { get; set; } = string.Empty;
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
}
