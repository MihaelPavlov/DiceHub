namespace DH.Domain.Entities;

public class EmailTemplate
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateHtml { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}
