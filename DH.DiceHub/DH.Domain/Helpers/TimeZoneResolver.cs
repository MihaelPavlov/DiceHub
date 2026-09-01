namespace DH.Domain.Helpers;

/// <summary>
/// Single source of truth for turning a stored IANA time zone id (e.g.
/// "Europe/Sofia") into a <see cref="TimeZoneInfo"/>. Everything that schedules
/// a job at a tenant-local wall-clock time - CloseActiveTablesJob, the weekly
/// reward reset, the per-tenant daily crons - resolves through here so the
/// fallback rule lives in one place.
///
/// Prod runs on Linux, whose native IANA tz database backs
/// <see cref="TimeZoneInfo.FindSystemTimeZoneById(string)"/> directly; .NET 8 on
/// Windows accepts the same IANA ids via ICU.
/// </summary>
public static class TimeZoneResolver
{
    /// <summary>
    /// Historical default - every DiceHub club was in Bulgaria when per-tenant
    /// time zones were introduced, and existing TenantSetting rows have a null
    /// TimeZoneId that must keep behaving exactly as before.
    /// </summary>
    public const string DefaultTimeZoneId = "Europe/Sofia";

    /// <summary>
    /// Resolves <paramref name="timeZoneId"/>, falling back to
    /// <see cref="DefaultTimeZoneId"/> when it is null, blank, or not a tz id the
    /// host recognises. Never throws.
    /// </summary>
    public static TimeZoneInfo Resolve(string? timeZoneId)
        => TryResolve(timeZoneId, out var timeZone) ? timeZone : GetDefault();

    /// <summary>
    /// True (with <paramref name="timeZone"/> set to the requested zone) when
    /// <paramref name="timeZoneId"/> is a valid, recognised id; false (with
    /// <paramref name="timeZone"/> set to the default zone) otherwise.
    /// </summary>
    public static bool TryResolve(string? timeZoneId, out TimeZoneInfo timeZone)
    {
        if (!string.IsNullOrWhiteSpace(timeZoneId))
        {
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                // fall through to the default
            }
        }

        timeZone = GetDefault();
        return false;
    }

    /// <summary>
    /// Whether <paramref name="timeZoneId"/> is a non-blank id the host can
    /// resolve. Used to validate owner input before it is persisted.
    /// </summary>
    public static bool IsValid(string? timeZoneId)
        => !string.IsNullOrWhiteSpace(timeZoneId) && TryResolve(timeZoneId, out _);

    private static TimeZoneInfo GetDefault()
        => TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
}
