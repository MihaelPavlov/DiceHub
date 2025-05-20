namespace DH.Domain.Entities;

public class EventNotification
{
    public int Id { get; set; }
    public DateTime SentOn { get; set; }
    public int RecipientCount { get; set; }

    public Event Event { get; set; } = null!;
}
