using System.Net;
using Microsoft.Extensions.Logging;
using SpaceTradersAPI.Repositories;
using static SpaceTradersAPI.Services.Helper;

namespace SpaceTradersAPI.Services;

public class SpaceTraders
{
    private readonly Client _client;
    private readonly ILogger _logger;
    private readonly MemoryShipLockRepository _shipLockRepository;
    private readonly ISystemService _systemService;

    public SpaceTraders(
        ILogger logger,
        Client client,
        ISystemService systemService,
        MemoryShipLockRepository shipLockRepository,
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

    public Task<List<Ship>> GetShips() => GetAll(_client.ListShips);

    public async Task<Waypoint> GetWaypointByType(string systemSymbol, WaypointType wpType) => (await _systemService.ListWaypoints(systemSymbol)).FirstOrDefault(wp => wp.Type == wpType);
    public async Task<Waypoint[]> GetWaypointsByTrait(string systemSymbol, TraitSymbol trait) => (await _systemService.ListWaypoints(systemSymbol)).Where(wp => wp.Traits.Any(t => t.Symbol == trait)).ToArray();

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
        var arrivalTtl = response.Data.Nav.Route.Arrival - DateTime.Now;
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

    public async Task Deliver(Contract contract, Ship ship, Cargo cargo)
    {
        if (ship.Nav.Status != NavStatus.DOCKED)
            throw new Exception("ship is not docked");

        var inv = cargo.GetResourcesMap();
        foreach (var toDeliver in contract.Terms.Deliver)
        {
            if (toDeliver.DestinationSymbol != ship.Nav.WaypointSymbol)
                continue;

            if (!inv.TryGetValue(toDeliver.TradeSymbol, out var units))
                continue;

            await _client.DeliverContract(contract.Id, ship.Symbol, toDeliver.TradeSymbol, units);
        }
    }

    public Task PurchaseCargo(Ship ship, int units, Trade trade) => _client.PurchaseCargo(ship.Symbol, trade, units);

    public async Task<Market> GetMarket(string systemSymbol, string waypointSymbol) => (await _client.GetMarket(systemSymbol, waypointSymbol)).Data;

    public async Task<string> GetMarketWpWithResource(string systemSymbol, Trade trade)
    {
        var wps = await GetWaypointsByTrait(systemSymbol, TraitSymbol.MARKETPLACE);
        foreach (var wp in wps)
        {
            var market = await _systemService.GetMarketWithoutTradingGoods(systemSymbol, wp.Symbol);
            if (market.Imports.Any(x => x.Symbol == trade))
                return market.Symbol;
        }

        return null;
    }
}