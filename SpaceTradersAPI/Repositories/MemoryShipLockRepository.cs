using System.Runtime.Caching;

namespace SpaceTradersAPI.Repositories;

public class MemoryShipLockRepository
{
    private readonly MemoryCache _cache;

    public MemoryShipLockRepository()
    {
        _cache = MemoryCache.Default;
    }

    public Task SetUnavailable(string shipSymbol, TimeSpan ttl)
    {
        _cache.Add(shipSymbol, 1, DateTime.Now.Add(ttl));
        return Task.CompletedTask;
    }

    public Task<bool> IsUnavailable(string shipSymbol)
    {
        var value = _cache.Get(shipSymbol);
        return value == null ? Task.FromResult(false) : Task.FromResult(true);
    }
}