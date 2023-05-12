using Microsoft.Extensions.Logging;
using SpaceTradersAPI;
using SpaceTradersAPI.Factories;
using SpaceTradersBot;

const string symbol = "Tyrael";
const Faction faction = Faction.COSMIC;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(options => options.FormatterName = nameof(CustomFormatter)).AddConsoleFormatter<CustomFormatter, CustomFormatterOptions>();
});

var accountService = new AccountService(loggerFactory.CreateLogger<AccountService>(), TokenRepoFactory.CreateFileTokenRepo(), new Client(HttpClientFactory.CreateClient()));
var token = await accountService.GetOrCreateToken(symbol, faction);

var service = ServiceFactory.CreateService(loggerFactory, symbol, token);
var bot = new Bot(loggerFactory, service);

var cts = new CancellationTokenSource();
await bot.Run(cts.Token);