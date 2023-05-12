using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI.Factories;

public static class ServiceFactory
{
    public static SpaceTraders CreateService(ILoggerFactory loggerFactory, string symbol, string token)
    {
        var client = new Client(HttpClientFactory.CreateClient());
        client.SetToken(token);
        return new SpaceTraders(loggerFactory.CreateLogger(symbol), client, symbol);
    }
}