namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetSpaceTableParticipantListQueryModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int SpaceTableId { get; set; }
    public DateTime JoinedAt { get; set; }
}
