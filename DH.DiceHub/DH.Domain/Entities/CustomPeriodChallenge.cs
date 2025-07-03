namespace DH.Domain.Entities;

public class CustomPeriodChallenge
{
    public int Id { get; set; }
    public int Attempts { get; set; }
    public int RewardPoints { get; set; }

    public int GameId { get; set; }
    public virtual Game Game { get; set; } = null!;
}
