using Microsoft.Extensions.Logging;
using SpaceTradersAPI;
using SpaceTradersAPI.Services;

namespace SpaceTradersBot;

internal class Bot
{
    private const string SystemSymbol = "X1-ZA40";
    private readonly ILogger _logger;
    private readonly SpaceTraders _service;

    public Bot(ILoggerFactory loggerFactory, SpaceTraders service)
    {
        _logger = loggerFactory.CreateLogger(service.Symbol);
        _service = service;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var agent = await _service.GetAgent();
        _logger.LogInformation($"Bot started:\n\tAccount: {_service.Symbol}\n\tCredits: {agent.Credits}\n\tHQ: {agent.Headquarters}");

        var lastPerformed = true;
        while (!cancellationToken.IsCancellationRequested)
        {
            var actionPerformed = await Resolve();
            if (!actionPerformed && lastPerformed) _logger.LogInformation("No action performed, waiting for the next action");
            lastPerformed = actionPerformed;

            await Task.Delay(TimeSpan.FromSeconds(!actionPerformed ? 3 : 1), cancellationToken);
        }
    }

    private async Task<bool> Resolve()
    {
        var agent = await _service.GetAgent();
        var contract = await GetCurrentContract();
        var contractResources = contract.Terms.Deliver.ToDictionary(x => x.TradeSymbol, x => x.UnitsRequired);
        var idleShips = await _service.GetIdleShips();
        if (idleShips.Count == 0)
            return false;

        if (agent.Credits > 100_000)
            await BuyShips(SystemSymbol, ShipType.SHIP_MINING_DRONE);

        foreach (var ship in idleShips)
        {
            if (ship.Nav.SystemSymbol != SystemSymbol)
            {
                _logger.LogWarning($"Ship {ship.Symbol} is not in the right system");
                continue;
            }

            var cargo = await _service.GetShipCargo(ship.Symbol);
            if (cargo.Full())
            {
                if (ship.Nav.Status == NavStatus.IN_ORBIT)
                    await _service.Dock(ship);

                var soldSomething = false;
                foreach (var inventory in cargo.Inventory)
                {
                    if (inventory.Symbol == Trade.ANTIMATTER)
                        continue;

                    if (contractResources.TryGetValue(inventory.Symbol, out var resource))
                    {
                        _logger.LogInformation($"Ship {ship.Symbol} cargo has {inventory.Units} of {inventory.Symbol} out of {resource} required");
                        continue;
                    }

                    await _service.SellCargo(ship.Symbol, inventory.Symbol, inventory.Units);
                    soldSomething = true;
                }

                if (!soldSomething)
                {
                    _logger.LogWarning($"Ship {ship.Symbol} is still full");
                    continue;
                }
            }

            if (ship.Nav.Status == NavStatus.DOCKED)
                await _service.Orbit(ship);

            var asteroid = await _service.GetWaypointByType(ship.Nav.SystemSymbol, WaypointType.ASTEROID_FIELD);
            if (asteroid.Symbol != ship.Nav.WaypointSymbol)
            {
                await _service.Navigate(ship, asteroid.Symbol);
                _logger.LogInformation($"Ship {ship.Symbol} Navigating to an asteroid");
                continue;
            }

            await _service.Extract(ship.Symbol);
        }

        return true;
    }

    private async Task BuyShips(string systemSymbol, ShipType shipType)
    {
        var shipyard = await _service.GetWaypointByTrait(systemSymbol, TraitSymbol.SHIPYARD);
        await _service.PurchaseShip(shipType, shipyard.Symbol);
    }

    private async Task<Contract> GetCurrentContract()
    {
        var contracts = await _service.GetContracts();
        var currentContract = contracts.FirstOrDefault(c => c.Accepted);
        if (currentContract == null)
        {
            var firstContract = contracts.First();
            var contractId = contracts.First().Id;
            await _service.AcceptContract(contractId);
            _logger.LogInformation($"Contract {contractId} accepted!");
            currentContract = firstContract;
        }

        return currentContract;
    }
}