using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI;

public class SpaceTraders
{
    private readonly Client _client;
    private readonly ILogger _logger;

    public SpaceTraders(
        ILogger logger,
        Client client,
        string symbol
    )
    {
        _logger = logger;
        _client = client;
        Symbol = symbol;
    }

    public string Symbol { get; }

    public async Task<Agent> GetAgent()
    {
        var data = await _client.AgentDetailsAsync();
        return data.Data;
    }

    private async Task<List<T>> GetAll<T>(Func<int, int, Task<DataGenerics<T[]>>> fetcher)
    {
        const int limit = 20;

        var element = new List<T>();
        var resp = await fetcher(1, limit);
        element.AddRange(resp.Data);
        for (var i = resp.Meta.Page + 1;; i++)
        {
            if (resp.Meta.Page * limit >= resp.Meta.Total)
                break;

            resp = await fetcher(i, limit);
            element.AddRange(resp.Data);
        }

        return element;
    }

    public Task<List<Ship>> GetShips()
    {
        return GetAll(_client.ListShips);
    }

    public Task<List<Waypoint>> GetWaypoints(string systemSymbol)
    {
        return GetAll((page, limit) => _client.ListWaypoints(systemSymbol, page, limit));
    }

    public async Task<Waypoint> GetClosestWaypoint(string systemSymbol, int x, int y, TraitSymbol? trait)
    {
        var waypoints = await GetWaypoints(systemSymbol);
        if (trait != null) waypoints = waypoints.Where(wp => wp.Traits.Any(t => t.Symbol == trait)).ToList();
        return waypoints.MinBy(wp => DistanceSquare(wp.X, wp.X, x, y));
    }

    private int DistanceSquare(int x1, int y1, int x2, int y2)
    {
        return x1 * x2 + y1 * y2;
    }

    public async Task<List<Ship>> GetAvailableShips()
    {
        var ships = await GetShips();

        return ships.Where(x => x.Nav.Status == NavStatus.DOCKED).ToList();
    }
}