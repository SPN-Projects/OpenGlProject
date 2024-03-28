namespace GameEngine.Logging;
public class Logger
{
    private static readonly object _lock = new();
    private readonly AsyncLocal<string> _projectNameContext = new();
    private readonly AsyncLocal<ConsoleColor> _projectColorContext = new();

    public static Logger EngineLogger { get; } = new("Engine", EngineConstants.ConsoleLoggerEngineProjectColor);
    public static Logger GameLogger { get; } = new("Game", EngineConstants.ConsoleLoggerGameProjectColor);

    public Logger(string projectName, ConsoleColor projectColor)
    {
        _projectNameContext.Value = projectName;
        _projectColorContext.Value = projectColor;
    }

    public void Trace(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerTraceColor, LoggingType.Trace, args);

    public void Debug(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerDebugColor, LoggingType.Debug, args);

    public void Info(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerInfoColor, LoggingType.Info, args);

    public void Warning(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerWarningColor, LoggingType.Warning, args);

    public void Error(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerErrorColor, LoggingType.Error, args);

    public void Critical(string message, params object[] args)
        => WriteConsole(message, EngineConstants.ConsoleLoggerCriticalColor, LoggingType.Critical, args);

    private void WriteConsole(string message, ConsoleColor color, LoggingType loggingType, params object[] args)
    {
        var projectName = _projectNameContext.Value ?? "Unknown";
        var projectColor = _projectColorContext.Value;

        lock (_lock)
        {
            Console.ForegroundColor = color;
            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.Write($"[{loggingType}] ");
            Console.ForegroundColor = projectColor;
            Console.Write($"[{projectName}] ");
            Console.ForegroundColor = color;
            Console.WriteLine(message);

            if (args.Length > 0)
            {
                Console.Write("Extras: ");
                Console.WriteLine(string.Join(", ", args.ToArray()));
            }

            Console.ResetColor();
        }
    }
}
