namespace DH.Domain.Entities;

public class EmailHistory
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public DateTime SendedOn { get; set; }
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsSuccessfully { get; set; }
}
