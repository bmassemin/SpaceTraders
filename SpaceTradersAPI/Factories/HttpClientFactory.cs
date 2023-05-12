namespace SpaceTradersAPI.Factories;

public static class HttpClientFactory
{
    private const string SpaceTradersUrl = "https://api.spacetraders.io";

    public static HttpClient CreateClient()
    {
        return new HttpClient(new HttpClientHandler())
        {
            BaseAddress = new Uri(SpaceTradersUrl)
        };
    }
}