using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace SpaceTradersBot;

//public class CustomLoggerFactory : ILoggerFactory
//{
//    public void Dispose()
//    {
//    }

//    public ILogger CreateLogger(string categoryName)
//    {
//        var f = LoggerFactory.Create(builder =>
//        {
//            builder.AddConsole().AddConsoleFormatter<CustomFormatter, CustomFormatterOptions>();
//        });
//        f.CreateLogger(categoryName);
//    }

//    public void AddProvider(ILoggerProvider provider)
//    {
//    }
//}

internal class CustomFormatter : ConsoleFormatter
{
    public CustomFormatter(IOptions<CustomFormatterOptions> _) : base(nameof(CustomFormatter))
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        textWriter.WriteLine($"[{logEntry.Category}] {message}");
    }
}

internal class CustomFormatterOptions : ConsoleFormatterOptions
{
}