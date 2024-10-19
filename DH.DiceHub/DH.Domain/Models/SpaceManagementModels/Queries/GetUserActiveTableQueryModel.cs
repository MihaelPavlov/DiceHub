namespace DH.Domain.Models.SpaceManagementModels.Queries;

public class GetUserActiveTableQueryModel
{
    public bool IsPlayerHaveActiveTable { get; set; }
    public bool IsPlayerParticipateInTable { get; set; }
    public string? ActiveTableName { get; set; }
    public int? ActiveTableId { get; set; }
}
