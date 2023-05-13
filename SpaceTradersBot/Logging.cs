using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace SpaceTradersBot;

internal class AccountLogger : ConsoleFormatter
{
    public AccountLogger() : base(nameof(AccountLogger))
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        Console.ForegroundColor = logEntry.LogLevel switch
        {
            LogLevel.Debug => ConsoleColor.DarkGray,
            LogLevel.Information => ConsoleColor.DarkGreen,
            LogLevel.Warning => ConsoleColor.DarkYellow,
            _ => Console.ForegroundColor
        };

        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        textWriter.WriteLine($"[{logEntry.Category}] {message}");
    }
}