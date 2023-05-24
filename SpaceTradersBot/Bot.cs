using Microsoft.Extensions.Logging;
using SpaceTradersAPI;
using SpaceTradersAPI.Services;

namespace SpaceTradersBot;

internal class Bot
{
    private const string SystemSymbol = "X1-VS75";
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
        var idleShips = await _service.GetIdleShips();
        if (idleShips.Count == 0)
            return false;

        if (agent.Credits > 100_000)
            await BuyShips(SystemSymbol, ShipType.SHIP_MINING_DRONE);

        var status = false;
        foreach (var ship in idleShips)
        {
            if (ship.Nav.SystemSymbol != SystemSymbol)
            {
                _logger.LogWarning($"Ship {ship.Symbol} is not in the right system");
                continue;
            }

            switch (ship.Registration.Role)
            {
                case ShipRole.COMMAND:
                    status = status || await ContractLoop(ship, contract, agent);
                    break;
                case ShipRole.EXCAVATOR:
                    status = status || await ExcavatorLoop(ship);
                    break;
            }
        }

        return status;
    }

    private async Task<bool> ContractLoop(Ship ship, Contract contract, Agent agent)
    {
        var term = contract.Terms.Deliver[0];
        var deliverWp = term.DestinationSymbol;
        var resource = term.TradeSymbol;
        var remainingUnits = term.UnitsRequired - term.UnitsFulfilled;

        var cargo = await _service.GetShipCargo(ship.Symbol);
        if (cargo.Full())
        {
            if (ship.Nav.WaypointSymbol == deliverWp)
            {
                if (ship.Nav.Status == NavStatus.IN_ORBIT)
                    await _service.Dock(ship);

                await _service.Deliver(contract, ship, cargo);
            }
            else
            {
                await Navigate(ship, deliverWp);
                return true;
            }
        }

        var marketWp = await _service.GetMarketWpWithResource(SystemSymbol, resource);
        if (marketWp == null) throw new Exception("Unable to find the required resource for the contract");

        if (marketWp != ship.Nav.WaypointSymbol)
        {
            await Navigate(ship, marketWp);
            return true;
        }

        if (ship.Nav.Status == NavStatus.IN_ORBIT)
            await _service.Dock(ship);

        var market = await _service.GetMarket(SystemSymbol, marketWp);
        var tradeGood = market.TradeGoods?.FirstOrDefault(x => x.Symbol == resource);
        if (tradeGood == null)
            throw new Exception("abnormal situation");

        var units = Math.Min(cargo.Free(), remainingUnits);
        if (tradeGood.PurchasePrice * units > agent.Credits)
            return false;

        await _service.PurchaseCargo(ship, units, resource);

        await ContractLoop(ship, contract, agent);

        return true;
    }

    private async Task<bool> ExcavatorLoop(Ship ship)
    {
        var cargo = await _service.GetShipCargo(ship.Symbol);
        if (cargo.Full())
        {
            if (ship.Nav.Status == NavStatus.IN_ORBIT)
                await _service.Dock(ship);

            foreach (var inventory in cargo.Inventory)
                await _service.SellCargo(ship.Symbol, inventory.Symbol, inventory.Units);
        }

        var asteroid = await _service.GetWaypointByType(ship.Nav.SystemSymbol, WaypointType.ASTEROID_FIELD);
        if (asteroid.Symbol != ship.Nav.WaypointSymbol)
        {
            await Navigate(ship, asteroid.Symbol);
            return true;
        }

        if (ship.Nav.Status == NavStatus.DOCKED)
            await _service.Orbit(ship);

        await _service.Extract(ship.Symbol);

        return true;
    }

    private async Task Navigate(Ship ship, string wp)
    {
        _logger.LogInformation($"Ship {ship.Symbol} Navigating to {wp}");

        if (ship.Nav.Status == NavStatus.DOCKED)
            await _service.Orbit(ship);

        await _service.Navigate(ship, wp);
    }

    private async Task BuyShips(string systemSymbol, ShipType shipType)
    {
        var shipyard = (await _service.GetWaypointsByTrait(systemSymbol, TraitSymbol.SHIPYARD)).FirstOrDefault();
        if (shipyard == null)
        {
            _logger.LogWarning("No shipyard found");
            return;
        }

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