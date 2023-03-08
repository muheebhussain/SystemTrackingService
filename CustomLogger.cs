public static class CustomLoggingBuilderExtensions
{
    public static ILoggingBuilder AddNamedLoggerProvider(this ILoggingBuilder builder, string serviceName)
    {
        return builder.AddProvider(new NamedLoggerProvider(serviceName));
    }
}
public class NamedLoggerProvider : ILoggerProvider
{
    private readonly string _serviceName;

    public NamedLoggerProvider(string serviceName)
    {
        _serviceName = serviceName;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new NamedLogger(categoryName, _serviceName);
    }

    public void Dispose()
    {
    }
}

public class NamedLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _serviceName;

    public NamedLogger(string categoryName, string serviceName)
    {
        _categoryName = categoryName;
        _serviceName = serviceName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public async void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        string message = formatter(state, exception);

        string processName = null;
        var scope = state as IReadOnlyList<KeyValuePair<string, object>>;
        if (scope != null)
        {
            foreach (var item in scope)
            {
                if (item.Key == "ProcessName" && item.Value is string)
                {
                    processName = (string)item.Value;
                    break;
                }
            }
        }

        await LogAsync(_serviceName, _categoryName, processName, logLevel, message);
    }

    private static async Task LogAsync(string serviceName, string categoryName, string processName, LogLevel logLevel, string message)
    {
        string logFileName = $"{serviceName}/{categoryName}/{processName ?? string.Empty}/{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log";
        string logFilePath = Path.Combine(Environment.CurrentDirectory, "logs", logFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
        using (var logFile = new StreamWriter(logFilePath, true))
        {
            await logFile.WriteLineAsync($"{DateTime.UtcNow.ToString("o")} [{logLevel}] {message}");
            await logFile.FlushAsync();
        }
    }
}

public class MyService
{
    private readonly ILogger _logger;

    public MyService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("MyService");
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("MyService started");

        _logger.LogInformation("This is a message from MyService");

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            { "ProcessName", "Process1" }
        }))
        {
            _logger.LogInformation("This is a message from MyService, Process1");

            // Call another service that logs messages
            var otherService = new OtherService(_logger);
            await otherService.RunAsync();
        }

        _logger.LogInformation("This is another message from MyService");

        _logger.LogInformation("MyService finished");
    }
}

public class OtherService
{
    private readonly ILogger _logger;

    public OtherService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("OtherService");
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("This is a message from OtherService");

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            { "ProcessName", "Process2" }
        }))
        {
            _logger.LogInformation("This is a message from OtherService, Process2");
        }
    }
}
.ConfigureServices((hostContext, services) =>
{
    // Register named logger providers
    string serviceName = nameof(MyService);
    services.AddLogging(logging => logging.AddNamedLoggerProvider(serviceName));
    services.AddSingleton<ILoggerFactory>(new LoggerFactory(new[] { new ConsoleLoggerProvider((category, logLevel) => logLevel >= LogLevel.Information, true) }));

    // Register services
    services.AddSingleton<MyService>();
    services.AddSingleton<OtherService>();
})
.UseConsoleLifetime();

