using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using SpaceTraders;
using SpaceTraders.API;

const string url = "https://api.spacetraders.io";
const string tokenFilename = "token";

var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Converters = { new StringEnumConverter() }
};

var retryHandler = new RetryHandler();
var httpClient = new HttpClient(retryHandler)
{
    BaseAddress = new Uri(url)
};
var client = RestService.For<IClient>(httpClient, settings);

string token;
if (File.Exists(tokenFilename))
{
    token = await File.ReadAllTextAsync(tokenFilename);
}
else
{
    var data = await client.RegisterNewAgentAsync(new RegisterNewAgentRequest
    {
        Faction = Faction.COSMIC,
        Symbol = "Tyrael"
    });
    token = data.Data.Token;
    await File.WriteAllTextAsync(tokenFilename, token);
}

httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var agent = await client.AgentDetailsAsync();
Console.WriteLine(
    @$"Successfully authenticated:
    Agent: {agent.Data.Symbol}
    Credits: {agent.Data.Credits}");