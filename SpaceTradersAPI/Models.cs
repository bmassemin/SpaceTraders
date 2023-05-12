namespace SpaceTradersAPI;

public enum Faction
{
    COSMIC,
    VOID,
    GALACTIC,
    QUANTUM,
    DOMINION
}

public class RegisterNewAgentRequest
{
    public string Symbol { get; set; }
    public Faction Faction { get; set; }
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
    IN_ORBIT
}

public enum WaypointType
{
    PLANET,
    ORBITAL_STATION,
    ASTEROID_FIELD
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

public class Contract
{
    public string Id { get; set; }
    public Faction FactionSymbol { get; set; }
    public ContractType Type { get; set; }
}

public enum TraitSymbol
{
    SHIPYARD
}

public class Trait
{
    public TraitSymbol Symbol { get; set; }
}

public class Waypoint
{
    public string SystemSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Symbol { get; set; }

    public Trait[] Traits { get; set; }
}