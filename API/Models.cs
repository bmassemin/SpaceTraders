using System.Runtime.Serialization;

namespace SpaceTraders.API;

internal enum Faction
{
    [EnumMember(Value = nameof(COSMIC))] COSMIC,
    [EnumMember(Value = nameof(VOID))] VOID,
    [EnumMember(Value = nameof(GALACTIC))] GALACTIC,
    [EnumMember(Value = nameof(QUANTUM))] QUANTUM,
    [EnumMember(Value = nameof(DOMINION))] DOMINION
}

internal class RegisterNewAgentRequest
{
    public string Symbol { get; set; }
    public Faction Faction { get; set; }
}

internal class DataResponse
{
    public Data Data { get; set; }
}

internal class Data
{
    public string Token { get; set; }
    public Agent Agent { get; set; }
}

internal class DataGenerics<T>
{
    public T Data { get; set; }
}

internal class Agent
{
    private string AccountId { get; set; }
    public string Symbol { get; set; }
    public string Headquarters { get; set; }
    public int Credits { get; set; }
}