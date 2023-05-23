using System.Net;
using Microsoft.Extensions.Logging;
using SpaceTradersAPI.Repositories;

namespace SpaceTradersAPI.Services;

public class SpaceTraders
{
    private readonly Client _client;
    private readonly ILogger _logger;
    private readonly ShipLockRepository _shipLockRepository;
    private readonly ISystemService _systemService;

    public SpaceTraders(
        ILogger logger,
        Client client,
        ISystemService systemService,
        ShipLockRepository shipLockRepository,
        string symbol
    )
    {
        _logger = logger;
        _client = client;
        _systemService = systemService;
        _shipLockRepository = shipLockRepository;
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

    public async Task<Waypoint> GetWaypointByType(string systemSymbol, WaypointType wpType) => (await _systemService.ListWaypoints(systemSymbol)).FirstOrDefault(wp => wp.Type == wpType);
    public async Task<Waypoint> GetWaypointByTrait(string systemSymbol, TraitSymbol trait) => (await _systemService.ListWaypoints(systemSymbol)).FirstOrDefault(wp => wp.Traits.Any(t => t.Symbol == trait));

    private async Task<bool> IsShipOnCooldown(string shipSymbol)
    {
        if (await _shipLockRepository.IsUnavailable(shipSymbol))
            return true;

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
        var response = await _client.Navigate(ship.Symbol, wpSymbol);
        ship.Nav.Status = NavStatus.IN_TRANSIT;
        var arrivalTtl = response.Data.Route.Arrival - DateTime.Now;
        await _shipLockRepository.SetUnavailable(ship.Symbol, arrivalTtl);
    }

    public async Task Extract(string shipSymbol)
    {
        _logger.LogDebug($"Ship {shipSymbol} is extracting resources");
        var extract = await _client.Extract(shipSymbol);
        await _shipLockRepository.SetUnavailable(shipSymbol, TimeSpan.FromSeconds(extract.Data.Cooldown.RemainingSeconds));
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