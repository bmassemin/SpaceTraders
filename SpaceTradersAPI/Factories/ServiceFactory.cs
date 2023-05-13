using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI.Factories;

public static class ServiceFactory
{
    public static SpaceTraders CreateService(ILoggerFactory accountLoggerFactory, string symbol, string token)
    {
        var client = new Client(HttpClientFactory.CreateClient(accountLoggerFactory.CreateLogger(symbol)));
        client.SetToken(token);
        return new SpaceTraders(accountLoggerFactory.CreateLogger(symbol), client, symbol);
    }
}