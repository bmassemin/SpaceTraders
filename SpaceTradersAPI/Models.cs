namespace SpaceTradersAPI;

public enum Faction
{
    COSMIC,
    VOID,
    GALACTIC,
    QUANTUM,
    DOMINION
}

public class DataResponse
{
    public Data Data { get; set; }
}

public class Data
{
    public string Token { get; set; }
    public Agent Agent { get; set; }
}

public class Meta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}

public class DataGenerics<T>
{
    public T Data { get; set; }
    public Meta Meta { get; set; }
}

public class Agent
{
    private string AccountId { get; set; }
    public string Symbol { get; set; }
    public string Headquarters { get; set; }
    public int Credits { get; set; }
}

public enum NavStatus
{
    DOCKED,
    IN_ORBIT,
    IN_TRANSIT
}

public enum WaypointType
{
    PLANET,
    MOON,
    ORBITAL_STATION,
    ASTEROID_FIELD,
    GAS_GIANT,
    JUMP_GATE
}

public class Position
{
    public string Symbol { get; set; }
    public WaypointType Type { get; set; }
    public string SystemSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class Route
{
    public Position Departure { get; set; }
    public Position Destination { get; set; }
}

public class Nav
{
    public string SystemSymbol { get; set; }
    public string WaypointSymbol { get; set; }
    public NavStatus Status { get; set; }
    public Route Route { get; set; }
}

public class Ship
{
    public string Symbol { get; set; }
    public Nav Nav { get; set; }
}

public enum ContractType
{
    PROCUREMENT
}

public enum Trade
{
    ALUMINUM_ORE,
    ICE_WATER,
    COPPER_ORE,
    SILICON_CRYSTALS,
    ANTIMATTER,
    QUARTZ_SAND,
    PLATINUM_ORE,
    GOLD_ORE,
    IRON_ORE,
    SILVER_ORE,
    AMMONIA_ICE
}

public class Deliver
{
    public Trade TradeSymbol { get; set; }
    public int UnitsRequired { get; set; }
}

public class Terms
{
    public Deliver[] Deliver { get; set; }
}

public class Contract
{
    public string Id { get; set; }
    public Faction FactionSymbol { get; set; }
    public ContractType Type { get; set; }
    public Terms Terms { get; set; }
    public bool Accepted { get; set; }
    public bool Fulfilled { get; set; }
}

public enum TraitSymbol
{
    OVERCROWDED,
    HIGH_TECH,
    BUREAUCRATIC,
    TEMPERATE,
    MARKETPLACE,
    BARREN,
    TRADING_HUB,
    VOLCANIC,
    FROZEN,
    TOXIC_ATMOSPHERE,
    WEAK_GRAVITY,
    MINERAL_DEPOSITS,
    COMMON_METAL_DEPOSITS,
    PRECIOUS_METAL_DEPOSITS,
    STRIPPED,
    VIBRANT_AURORAS,
    STRONG_MAGNETOSPHERE,
    MILITARY_BASE,
    DRY_SEABEDS,
    SHIPYARD
}

public class Trait
{
    public TraitSymbol Symbol { get; set; }
}

public enum SystemType
{
    NEUTRON_STAR
}

public class System
{
    public string Symbol { get; set; }
    public string SectorSymbol { get; set; }
    public SystemType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class Waypoint
{
    public string SystemSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Symbol { get; set; }
    public WaypointType Type { get; set; }

    public Trait[] Traits { get; set; }
}

public class Inventory
{
    public Trade Symbol { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Units { get; set; }
}

public class Cargo
{
    public int Capacity { get; set; }
    public int Units { get; set; }
    public Inventory[] Inventory { get; set; }

    public Dictionary<Trade, int> GetResourcesMap() => Inventory.ToDictionary(i => i.Symbol, i => i.Units);
    public bool Full() => Capacity == Units;
}

public enum ShipType
{
    SHIP_MINING_DRONE
}