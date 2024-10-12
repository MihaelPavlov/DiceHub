namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetSpaceTableListQueryModel
{
    public int Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public int GameImageId { get; set; }
    public int PeopleJoined { get; set; }
    public int MaxPeople { get; set; }
    public bool IsLocked { get; set; }
}
