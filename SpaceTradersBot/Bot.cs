using Microsoft.Extensions.Logging;
using SpaceTradersAPI;

namespace SpaceTradersBot;

internal class Bot
{
    private readonly ILogger _logger;
    private readonly SpaceTraders _service;

    public Bot(ILoggerFactory loggerFactory, SpaceTraders service)
    {
        _logger = loggerFactory.CreateLogger(service.Symbol);
        _service = service;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var agent = await _service.GetAgent();
        _logger.LogInformation($"Bot started:\n\tAccount: {_service.Symbol}\n\tCredits: {agent.Credits}\n\tHQ: {agent.Headquarters}");

        while (!cancellationToken.IsCancellationRequested)
        {
            await Resolve(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }

    private async Task Resolve(CancellationToken cancellationToken)
    {
        var availableShips = await _service.GetAvailableShips();
        if (availableShips.Count == 0)
        {
            _logger.LogInformation("No ship available for a mission");
        }
    }
}