using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceTradersAPI;

public class Client
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _defaultSerializationOptions;

    public Client(HttpClient client)
    {
        _client = client;
        _defaultSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<DataResponse> RegisterNewAgentAsync(RegisterNewAgentRequest request)
    {
        var resp = await _client.PostAsJsonAsync("/v2/register", request, _defaultSerializationOptions);
        return await resp.Content.ReadFromJsonAsync<DataResponse>(_defaultSerializationOptions);
    }

    public async Task<DataGenerics<Agent>> AgentDetailsAsync()
    {
        return await _client.GetFromJsonAsync<DataGenerics<Agent>>("/v2/my/agent", _defaultSerializationOptions);
    }

    public async Task<DataGenerics<Ship[]>> ListShips(int page, int limit = 20)
    {
        return await _client.GetFromJsonAsync<DataGenerics<Ship[]>>("/v2/my/ships", _defaultSerializationOptions);
    }

    public async Task<DataGenerics<Contract[]>> ListContracts(int page, int limit = 20)
    {
        return await _client.GetFromJsonAsync<DataGenerics<Contract[]>>("/v2/my/contracts", _defaultSerializationOptions);
    }

    public async Task<DataGenerics<Waypoint[]>> ListWaypoints(string systemSymbol, int page, int limit = 20)
    {
        return await _client.GetFromJsonAsync<DataGenerics<Waypoint[]>>($"/v2/my/systems/{systemSymbol}/waypoints", _defaultSerializationOptions);
    }
}