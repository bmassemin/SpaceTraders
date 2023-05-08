using System.Net;

namespace SpaceTraders;

internal class RetryHandler : DelegatingHandler
{
    public RetryHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return response;

        while (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            await Task.Delay(1000, cancellationToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}