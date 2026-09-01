namespace DH.Domain.Entities;

public class EmailTemplate : TenantEntity
{
    public int Id { get; set; }
    public string Language { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateHtml { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
}
