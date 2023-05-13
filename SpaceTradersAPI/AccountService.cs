using Microsoft.Extensions.Logging;

namespace SpaceTradersAPI;

public class AccountService
{
    private readonly Client _client;
    private readonly ILogger<AccountService> _logger;
    private readonly ITokenRepository _tokenRepository;

    public AccountService(ILogger<AccountService> logger, ITokenRepository tokenRepository, Client client)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _client = client;
    }

    public async Task<string> GetOrCreateToken(string symbol, Faction faction)
    {
        string token;
        if (!_tokenRepository.Exists(symbol))
        {
            var data = await _client.RegisterNewAgentAsync(symbol, faction);
            token = data.Data.Token;
            await _tokenRepository.SaveTokenAsync(symbol, token);
            _logger.LogInformation($"Account {symbol} registered");
        }
        else
        {
            token = await _tokenRepository.GetTokenAsync(symbol);
        }

        return token;
    }
}