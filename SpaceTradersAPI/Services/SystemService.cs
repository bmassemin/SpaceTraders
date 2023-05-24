using static SpaceTradersAPI.Services.Helper;

namespace SpaceTradersAPI.Services;

public interface ISystemService
{
    Task<List<Waypoint>> ListWaypoints(string systemSymbol);
    Task<System> GetSystem(string systemSymbol);
    Task<Market> GetMarketWithoutTradingGoods(string systemSymbol, string waypointSymbol);
}

public class SystemService : ISystemService
{
    private readonly Client _client;

    public SystemService(Client client)
    {
        _client = client;
    }

    public Task<List<Waypoint>> ListWaypoints(string systemSymbol) => GetAll((page, limit) => _client.ListWaypoints(systemSymbol, page, limit));

    public async Task<System> GetSystem(string systemSymbol) => (await _client.GetSystem(systemSymbol)).Data;

    public async Task<Market> GetMarketWithoutTradingGoods(string systemSymbol, string waypointSymbol)
    {
        var market = (await _client.GetMarket(systemSymbol, waypointSymbol)).Data;
        market.TradeGoods = null;
        return market;
    }
}