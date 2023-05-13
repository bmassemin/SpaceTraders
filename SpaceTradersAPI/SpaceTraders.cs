using System.Net;
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

    public async Task<List<T>> GetAll<T>(Func<int, int, Task<DataGenerics<T[]>>> fetcher)
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

    public Task<List<Ship>> GetShips() => GetAll(_client.ListShips);

    public Task<List<Waypoint>> GetWaypoints(string systemSymbol) => GetAll((page, limit) => _client.ListWaypoints(systemSymbol, page, limit));

    public async Task<Waypoint> GetWaypointByType(string systemSymbol, WaypointType wpType) => (await GetWaypoints(systemSymbol)).FirstOrDefault(wp => wp.Type == wpType);
    public async Task<Waypoint> GetWaypointByTrait(string systemSymbol, TraitSymbol trait) => (await GetWaypoints(systemSymbol)).FirstOrDefault(wp => wp.Traits.Any(t => t.Symbol == trait));

    private async Task<bool> IsShipOnCooldown(string shipSymbol)
    {
        var resp = await _client.GetShipCooldown(shipSymbol);
        return resp.StatusCode == HttpStatusCode.OK;
    }

    public async Task<List<Ship>> GetIdleShips()
    {
        var ships = await GetShips();

        ships = ships.Where(x => x.Nav.Status != NavStatus.IN_TRANSIT).ToList();

        var result = new List<Ship>();

        foreach (var ship in ships)
            if (!await IsShipOnCooldown(ship.Symbol))
                result.Add(ship);

        return result;
    }

    public Task<List<Contract>> GetContracts() => GetAll(_client.ListContracts);

    public Task AcceptContract(string contractId)
    {
        _logger.LogDebug($"contract {contractId} accepted!");
        return _client.AcceptContact(contractId);
    }

    public async Task Navigate(Ship ship, string wpSymbol)
    {
        _logger.LogDebug($"Ship {ship.Symbol} is navigating to {wpSymbol}");
        await _client.Navigate(ship.Symbol, wpSymbol);
        ship.Nav.Status = NavStatus.IN_TRANSIT;
    }

    public Task Extract(string shipSymbol)
    {
        _logger.LogDebug($"Ship {shipSymbol} is extracting resources");
        return _client.Extract(shipSymbol);
    }

    public async Task<Cargo> GetShipCargo(string shipSymbol)
    {
        var cargo = await _client.GetShipCargo(shipSymbol);
        return cargo.Data;
    }

    public async Task Dock(Ship ship)
    {
        _logger.LogDebug($"Ship {ship.Symbol} is docking");
        await _client.Dock(ship.Symbol);
        ship.Nav.Status = NavStatus.DOCKED;
    }

    public Task SellCargo(string shipSymbol, Trade trade, int units)
    {
        _logger.LogDebug($"Ship {shipSymbol} selling {units} units of {trade}");
        return _client.SellCargo(shipSymbol, trade, units);
    }

    public async Task Orbit(Ship ship)
    {
        _logger.LogDebug($"Ship {ship.Symbol} is going into orbit");
        await _client.Orbit(ship.Symbol);
        ship.Nav.Status = NavStatus.IN_ORBIT;
    }

    public Task PurchaseShip(ShipType shipType, string waypoint) => _client.PurchaseShip(shipType, waypoint);
}