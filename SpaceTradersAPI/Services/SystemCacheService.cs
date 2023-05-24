namespace SpaceTradersAPI.Services;

public class SystemCacheService : ISystemService
{
    private readonly Dictionary<string, Market> _marketCache = new();
    private readonly Dictionary<string, System> _systemCache = new();
    private readonly SystemService _systemService;
    private readonly Dictionary<string, List<Waypoint>> _waypointsCache = new();

    public SystemCacheService(SystemService systemService)
    {
        _systemService = systemService;
    }

    public async Task<List<Waypoint>> ListWaypoints(string systemSymbol)
    {
        if (_waypointsCache.TryGetValue(systemSymbol, out var waypoints))
            return waypoints;

        waypoints = await _systemService.ListWaypoints(systemSymbol);
        _waypointsCache[systemSymbol] = waypoints;
        return waypoints;
    }

    public async Task<System> GetSystem(string systemSymbol)
    {
        if (_systemCache.TryGetValue(systemSymbol, out var system))
            return system;

        system = await _systemService.GetSystem(systemSymbol);
        _systemCache[systemSymbol] = system;
        return system;
    }

    public async Task<Market> GetMarketWithoutTradingGoods(string systemSymbol, string waypointSymbol)
    {
        if (_marketCache.TryGetValue(waypointSymbol, out var market))
            return market;

        market = await _systemService.GetMarketWithoutTradingGoods(systemSymbol, waypointSymbol);
        _marketCache[waypointSymbol] = market;
        return market;
    }
}