using System.Net;

namespace SpaceTradersAPI;

internal class RetryHandler : DelegatingHandler
{
    public RetryHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            await Task.Delay(1000, cancellationToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}