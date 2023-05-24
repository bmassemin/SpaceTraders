using SpaceTradersAPI.Repositories;
using StackExchange.Redis;

namespace SpaceTradersAPI.Tests;

public class ShipLockTest
{
    private readonly IDatabase _db;

    public ShipLockTest()
    {
        var host = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        var redis = ConnectionMultiplexer.Connect(host);
        _db = redis.GetDatabase();
    }

    [Fact]
    public async Task TestSetUnavailable()
    {
        var locker = new RedisShipLockRepository(_db);

        await locker.SetUnavailable("TestSetUnavailable", TimeSpan.MaxValue);
        var value = await _db.StringGetAsync("TestSetUnavailable");

        Assert.Equal("1", value);
    }

    [Fact]
    public async Task TestIsUnavailable()
    {
        var locker = new RedisShipLockRepository(_db);

        await locker.SetUnavailable("TestIsUnavailable", TimeSpan.MaxValue);
        var value = await locker.IsUnavailable("TestIsUnavailable");

        Assert.True(value);
    }

    [Fact]
    public async Task TestIsUnavailable_Available()
    {
        var locker = new RedisShipLockRepository(_db);

        var value = await locker.IsUnavailable("TestIsUnavailable_Available");

        Assert.False(value);
    }
}