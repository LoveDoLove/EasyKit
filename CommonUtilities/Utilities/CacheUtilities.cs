using System.Runtime.Caching;

namespace CommonUtilities.Utilities;

public static class CacheUtilities
{
    private static readonly ObjectCache Cache = MemoryCache.Default;

    public static T GetOrAdd<T>(string key, Func<T> valueFactory, TimeSpan expiration)
    {
        if (Cache.Contains(key)) return (T)Cache[key];

        var value = valueFactory();
        Cache.Add(key, value, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) });
        return value;
    }

    public static void Remove(string key)
    {
        if (Cache.Contains(key)) Cache.Remove(key);
    }

    public static void ClearAll()
    {
        var cacheKeys = new List<string>(Cache.Select(kvp => kvp.Key));
        foreach (var key in cacheKeys) Cache.Remove(key);
    }
}