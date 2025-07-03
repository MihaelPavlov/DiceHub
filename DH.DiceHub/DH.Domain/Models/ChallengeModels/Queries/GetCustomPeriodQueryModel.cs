namespace DH.Domain.Models.ChallengeModels.Queries;

public class GetCustomPeriodQueryModel
{
    public List<GetCustomPeriodRewardQueryModel> Rewards { get; set; } = [];
    public List<GetCustomPeriodChallengeQueryModel> Challenges { get; set; } = [];
}

public class GetCustomPeriodRewardQueryModel
{
    public int Id { get; set; }
    public int SelectedReward { get; set; }
    public int RequiredPoints { get; set; }
}

public class GetCustomPeriodChallengeQueryModel
{
    public int Id { get; set; }
    public int SelectedGame { get; set; }
    public int Attempts { get; set; }
    public int Points { get; set; }
}
