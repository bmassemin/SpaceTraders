using Microsoft.Extensions.Logging;
using SpaceTradersAPI.HttpHandlers;

namespace SpaceTradersAPI.Factories;

public static class HttpClientFactory
{
    private const string SpaceTradersUrl = "https://api.spacetraders.io";

    public static HttpClient CreateClient(ILogger logger) => new(new ExceptionHandler(new RetryHandler(new HttpClientHandler(), logger), logger))
    {
        BaseAddress = new Uri(SpaceTradersUrl)
    };
}