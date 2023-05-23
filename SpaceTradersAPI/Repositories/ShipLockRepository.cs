using StackExchange.Redis;

namespace SpaceTradersAPI.Repositories;

public class ShipLockRepository
{
    private readonly IDatabase _redis;

    public ShipLockRepository(IDatabase redis)
    {
        _redis = redis;
    }

    public Task SetUnavailable(string shipSymbol, TimeSpan ttl) => _redis.StringSetAsync(shipSymbol, "1", ttl);

    public async Task<bool> IsUnavailable(string shipSymbol) => await _redis.StringGetAsync(shipSymbol) == 1;
}