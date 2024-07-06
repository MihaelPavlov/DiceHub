using DH.Domain.Adapters.Authentication.Interfaces;
using DH.Domain.Adapters.Authentication.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace DH.Adapter.Authentication.Services;

public class PermissionStringBuilder : IPermissionStringBuilder
{
    readonly IMapPermissions _mapPermissions;
    readonly IMemoryCache _cache;

    public PermissionStringBuilder(IMapPermissions mapPermissions, IMemoryCache cache)
    {
        _mapPermissions = mapPermissions;
        _cache = cache;
    }

    public string BuildPermissionsString(int roleKey)
    {
        var map = _mapPermissions.GetActionsMapping();
        var maxValue = map.Keys.Max();
        var sb = new StringBuilder(new string(' ', maxValue + 1));

        foreach (var item in map)
        {
            if (item.Value.Cast<int>().Contains(roleKey))
            {
                sb.Remove(item.Key, 1);
                sb.Insert(item.Key, "1");
            }
        }

        return sb.ToString();
    }

    public string GetFromCacheOrBuildPermissionsString(int roleKey)
    {
        var key = $"PerStr:{roleKey}";
        return _cache.GetOrCreate(key, (c) => BuildPermissionsString(roleKey))
            ?? BuildPermissionsString(roleKey);
    }
}
