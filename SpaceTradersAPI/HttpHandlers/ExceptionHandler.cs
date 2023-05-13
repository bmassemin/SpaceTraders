using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI.HttpHandlers;

internal class ExceptionHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    public ExceptionHandler(HttpMessageHandler parent, ILogger logger)
    {
        _logger = logger;
        InnerHandler = parent;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError($"Unexpected error {response.StatusCode}: {content}");
            Environment.Exit(1);
        }

        return response;
    }
}