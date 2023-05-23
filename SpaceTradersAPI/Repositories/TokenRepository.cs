namespace SpaceTradersAPI;

public interface ITokenRepository
{
    bool Exists(string symbol);
    Task SaveTokenAsync(string symbol, string token);
    Task<string> GetTokenAsync(string symbol);
}

public class FileTokenRepository : ITokenRepository
{
    private const string TokenPrefix = "token_";

    public bool Exists(string symbol)
    {
        return File.Exists(TokenPrefix + symbol);
    }

    public Task SaveTokenAsync(string symbol, string token)
    {
        return File.WriteAllTextAsync(TokenPrefix + symbol, token);
    }

    public Task<string> GetTokenAsync(string symbol)
    {
        return File.ReadAllTextAsync(TokenPrefix + symbol);
    }
}