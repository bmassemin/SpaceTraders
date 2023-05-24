namespace SpaceTradersAPI;

public enum Faction
{
    COSMIC,
    VOID,
    GALACTIC,
    QUANTUM,
    DOMINION
}

public class DataRegister
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
    GAS_GIANT,
    MOON,
    ORBITAL_STATION,
    JUMP_GATE,
    ASTEROID_FIELD,
    NEBULA,
    DEBRIS_FIELD,
    GRAVITY_WELL
}

public class DataNavigate
{
    public Nav Nav { get; set; }
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
    public DateTime DepartureTime { get; set; }
    public DateTime Arrival { get; set; }
}

public class Nav
{
    public string SystemSymbol { get; set; }
    public string WaypointSymbol { get; set; }
    public NavStatus Status { get; set; }
    public Route Route { get; set; }
}

public enum ShipRole
{
    COMMAND,
    EXCAVATOR
}

public class Registration
{
    public string Name { get; set; }
    public Faction FactionSymbol { get; set; }
    public ShipRole Role { get; set; }
}

public class Ship
{
    public string Symbol { get; set; }
    public Nav Nav { get; set; }
    public Registration Registration { get; set; }
}

public enum ContractType
{
    PROCUREMENT
}

public enum Trade
{
    PRECIOUS_STONES,
    QUARTZ_SAND,
    SILICON_CRYSTALS,
    AMMONIA_ICE,
    LIQUID_HYDROGEN,
    LIQUID_NITROGEN,
    ICE_WATER,
    EXOTIC_MATTER,
    ADVANCED_CIRCUITRY,
    GRAVITON_EMITTERS,
    IRON,
    IRON_ORE,
    COPPER,
    COPPER_ORE,
    ALUMINUM,
    ALUMINUM_ORE,
    SILVER,
    SILVER_ORE,
    GOLD,
    GOLD_ORE,
    PLATINUM,
    PLATINUM_ORE,
    DIAMONDS,
    URANITE,
    URANITE_ORE,
    MERITIUM,
    MERITIUM_ORE,
    HYDROCARBON,
    ANTIMATTER,
    FERTILIZERS,
    FABRICS,
    FOOD,
    JEWELRY,
    MACHINERY,
    FIREARMS,
    ASSAULT_RIFLES,
    MILITARY_EQUIPMENT,
    EXPLOSIVES,
    LAB_INSTRUMENTS,
    AMMUNITION,
    ELECTRONICS,
    SHIP_PLATING,
    EQUIPMENT,
    FUEL,
    MEDICINE,
    DRUGS,
    CLOTHING,
    MICROPROCESSORS,
    PLASTICS,
    POLYNUCLEOTIDES,
    BIOCOMPOSITES,
    NANOBOTS,
    AI_MAINFRAMES,
    QUANTUM_DRIVES,
    ROBOTIC_DRONES,
    CYBER_IMPLANTS,
    GENE_THERAPEUTICS,
    NEURAL_CHIPS,
    MOOD_REGULATORS,
    VIRAL_AGENTS,
    MICRO_FUSION_GENERATORS,
    SUPERGRAINS,
    LASER_RIFLES,
    HOLOGRAPHICS,
    SHIP_SALVAGE,
    RELIC_TECH,
    NOVEL_LIFEFORMS,
    BOTANICAL_SPECIMENS,
    CULTURAL_ARTIFACTS,
    REACTOR_SOLAR_I,
    REACTOR_FUSION_I,
    REACTOR_FISSION_I,
    REACTOR_CHEMICAL_I,
    REACTOR_ANTIMATTER_I,
    ENGINE_IMPULSE_DRIVE_I,
    ENGINE_ION_DRIVE_I,
    ENGINE_ION_DRIVE_II,
    ENGINE_HYPER_DRIVE_I,
    MODULE_MINERAL_PROCESSOR_I,
    MODULE_CARGO_HOLD_I,
    MODULE_CREW_QUARTERS_I,
    MODULE_ENVOY_QUARTERS_I,
    MODULE_PASSENGER_CABIN_I,
    MODULE_MICRO_REFINERY_I,
    MODULE_ORE_REFINERY_I,
    MODULE_FUEL_REFINERY_I,
    MODULE_SCIENCE_LAB_I,
    MODULE_JUMP_DRIVE_I,
    MODULE_JUMP_DRIVE_II,
    MODULE_JUMP_DRIVE_III,
    MODULE_WARP_DRIVE_I,
    MODULE_WARP_DRIVE_II,
    MODULE_WARP_DRIVE_III,
    MODULE_SHIELD_GENERATOR_I,
    MODULE_SHIELD_GENERATOR_II,
    MOUNT_GAS_SIPHON_I,
    MOUNT_GAS_SIPHON_II,
    MOUNT_GAS_SIPHON_III,
    MOUNT_SURVEYOR_I,
    MOUNT_SURVEYOR_II,
    MOUNT_SURVEYOR_III,
    MOUNT_SENSOR_ARRAY_I,
    MOUNT_SENSOR_ARRAY_II,
    MOUNT_SENSOR_ARRAY_III,
    MOUNT_MINING_LASER_I,
    MOUNT_MINING_LASER_II,
    MOUNT_MINING_LASER_III,
    MOUNT_LASER_CANNON_I,
    MOUNT_MISSILE_LAUNCHER_I,
    MOUNT_TURRET_I
}

public class Deliver
{
    public Trade TradeSymbol { get; set; }
    public string DestinationSymbol { get; set; }
    public int UnitsRequired { get; set; }
    public int UnitsFulfilled { get; set; }
}

public class Payment
{
    public int OnAccepted { get; set; }
    public int OnFulfilled { get; set; }
}

public class Terms
{
    public Deliver[] Deliver { get; set; }
    public Payment Payment { get; set; }
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

    public int Free() => Capacity - Units;
}

public enum ShipType
{
    SHIP_MINING_DRONE
}

public class Cooldown
{
    public string ShipSymbol { get; set; }
    public int TotalSeconds { get; set; }
    public int RemainingSeconds { get; set; }
    public DateTime Expiration { get; set; }
}

public class ExtractResponse
{
    public Cooldown Cooldown { get; set; }
}

public class Import
{
    public Trade Symbol { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public enum TransactionType
{
    PURCHASE,
    SELL
}

public class Transaction
{
    public string WaypointSymbol { get; set; }
    public Trade TradeSymbol { get; set; }
    public TransactionType Type { get; set; }
    public int Units { get; set; }
    public int PricePerUnit { get; set; }
    public int TotalPrice { get; set; }
}

public class TradeGood
{
    public Trade Symbol { get; set; }
    public int TradeVolume { get; set; }
    public int PurchasePrice { get; set; }
    public int SellPrice { get; set; }
}

public class Market
{
    public string Symbol { get; set; }
    public Import[] Imports { get; set; }
    public Transaction[] Transactions { get; set; }
    public TradeGood[] TradeGoods { get; set; }
}