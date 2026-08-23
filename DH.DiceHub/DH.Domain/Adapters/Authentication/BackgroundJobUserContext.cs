namespace DH.Domain.Adapters.Authentication;

/// <summary>
/// System-level user context used to scope a single tenant's data for background
/// jobs and workers (Quartz jobs, hosted services) that run outside an HTTP request
/// and therefore have no ambient tenant from the request pipeline.
/// </summary>
public sealed class BackgroundJobUserContext : IUserContext
{
    public BackgroundJobUserContext(string tenantId)
    {
        TenantId = tenantId;
    }

    public string? TenantId { get; }
    public string? UserId => "background-job";
    public int? RoleKey => null;
    public string? TimeZone => "UTC";
    public string? Language => "en";
    public bool IsAuthenticated => false;
    public bool IsSystem => true;
}
