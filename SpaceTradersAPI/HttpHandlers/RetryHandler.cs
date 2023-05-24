using System.Net;
using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI.HttpHandlers;

internal class RetryHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    public RetryHandler(HttpMessageHandler parent, ILogger logger)
    {
        _logger = logger;
        InnerHandler = parent;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning($"[{request.RequestUri}] Too many requests, throttling...");
            await Task.Delay(1000, cancellationToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}