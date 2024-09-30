namespace DH.Domain.Entities;

public class UserStatistic
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int TotalChallengesCompleted { get; set; }
}
