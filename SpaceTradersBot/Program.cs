using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SpaceTradersAPI;
using SpaceTradersAPI.Factories;
using SpaceTradersBot;

const string symbol = "Tyrael";
const Faction faction = Faction.COSMIC;

var accountLoggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(options => options.FormatterName = nameof(AccountLogger)).AddConsoleFormatter<AccountLogger, ConsoleFormatterOptions>();
    builder.SetMinimumLevel(LogLevel.Debug);
});

var accountService = new AccountService(
    accountLoggerFactory.CreateLogger<AccountService>(),
    TokenRepoFactory.CreateFileTokenRepo(),
    new Client(HttpClientFactory.CreateClient(accountLoggerFactory.CreateLogger("AccountService")))
);

var token = await accountService.GetOrCreateToken(symbol, faction);

var service = ServiceFactory.CreateService(accountLoggerFactory, symbol, token);
var bot = new Bot(accountLoggerFactory, service);

var cts = new CancellationTokenSource();
await bot.Run(cts.Token);