﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceTradersAPI;

public class Client
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _defaultSerializationOptions;

    public Client()
    {
    }

    public Client(HttpClient client)
    {
        _client = client;
        _defaultSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public void SetToken(string token) => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public async Task<DataGenerics<DataRegister>> RegisterNewAgentAsync(string symbol, Faction faction)
    {
        var resp = await _client.PostAsJsonAsync("/v2/register", new
        {
            Symbol = symbol,
            Faction = faction
        }, _defaultSerializationOptions);
        return await resp.Content.ReadFromJsonAsync<DataGenerics<DataRegister>>(_defaultSerializationOptions);
    }

    public Task<DataGenerics<Agent>> AgentDetailsAsync() => _client.GetFromJsonAsync<DataGenerics<Agent>>("/v2/my/agent", _defaultSerializationOptions);

    public Task<DataGenerics<Ship[]>> ListShips(int page, int limit = 20) => _client.GetFromJsonAsync<DataGenerics<Ship[]>>("/v2/my/ships", _defaultSerializationOptions);

    public Task<DataGenerics<Contract[]>> ListContracts(int page, int limit = 20) => _client.GetFromJsonAsync<DataGenerics<Contract[]>>("/v2/my/contracts", _defaultSerializationOptions);

    public Task<DataGenerics<Waypoint[]>> ListWaypoints(string systemSymbol, int page, int limit = 20) => _client.GetFromJsonAsync<DataGenerics<Waypoint[]>>($"/v2/systems/{systemSymbol}/waypoints", _defaultSerializationOptions);

    public Task<DataGenerics<System>> GetSystem(string systemSymbol) => _client.GetFromJsonAsync<DataGenerics<System>>($"/v2/systems/{systemSymbol}", _defaultSerializationOptions);

    public Task AcceptContact(string contractId) => _client.PostAsync($"/v2/my/contracts/{contractId}/accept", null);

    public async Task<DataGenerics<DataNavigate>> Navigate(string shipSymbol, string wpSymbol)
    {
        var response = await _client.PostAsJsonAsync($"/v2/my/ships/{shipSymbol}/navigate", new
        {
            WaypointSymbol = wpSymbol
        }, _defaultSerializationOptions);

        return await response.Content.ReadFromJsonAsync<DataGenerics<DataNavigate>>(_defaultSerializationOptions);
    }

    public async Task<DataGenerics<ExtractResponse>> Extract(string shipSymbol)
    {
        var response = await _client.PostAsync($"/v2/my/ships/{shipSymbol}/extract", null);
        return await response.Content.ReadFromJsonAsync<DataGenerics<ExtractResponse>>(_defaultSerializationOptions);
    }

    public Task<HttpResponseMessage> GetShipCooldown(string shipSymbol) => _client.GetAsync($"/v2/my/ships/{shipSymbol}/cooldown");

    public Task<DataGenerics<Cargo>> GetShipCargo(string shipSymbol) => _client.GetFromJsonAsync<DataGenerics<Cargo>>($"/v2/my/ships/{shipSymbol}/cargo", _defaultSerializationOptions);

    public Task Dock(string shipSymbol) => _client.PostAsync($"/v2/my/ships/{shipSymbol}/dock", null);

    public Task SellCargo(string shipSymbol, Trade trade, int units) => _client.PostAsJsonAsync($"/v2/my/ships/{shipSymbol}/sell", new
    {
        Symbol = trade,
        Units = units
    }, _defaultSerializationOptions);

    public Task Orbit(string shipSymbol) => _client.PostAsync($"/v2/my/ships/{shipSymbol}/orbit", null);

    public Task<HttpResponseMessage> PurchaseShip(ShipType shipType, string waypoint) => _client.PostAsJsonAsync("/v2/my/ships", new
    {
        ShipType = shipType,
        WaypointSymbol = waypoint
    }, _defaultSerializationOptions);

    public Task DeliverContract(string contractId, string shipSymbol, Trade trade, int units) => _client.PostAsJsonAsync($"/v2/my/contracts/{contractId}/deliver", new
    {
        ShipSymbol = shipSymbol,
        TradeSymbol = trade,
        Units = units
    }, _defaultSerializationOptions);

    public Task PurchaseCargo(string shipSymbol, Trade trade, int units) => _client.PostAsJsonAsync($"/v2/my/ships/{shipSymbol}/purchase", new
    {
        Symbol = trade,
        Units = units
    }, _defaultSerializationOptions);

    public Task<DataGenerics<Market>> GetMarket(string systemSymbol, string waypointSymbol) => _client.GetFromJsonAsync<DataGenerics<Market>>($"/v2/systems/{systemSymbol}/waypoints/{waypointSymbol}/market", _defaultSerializationOptions);
}