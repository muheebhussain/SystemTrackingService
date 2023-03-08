using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureBlob;
using System.Threading.Tasks;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure Azure Blob Storage logger
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddAzureBlobStorage(hostContext.Configuration.GetConnectionString("AzureBlobStorage"), "logs");
        });

        // Register services
        services.AddSingleton<MyService>();
    })
    .ConfigureLogging(logging =>
    {
        // Add console logger
        logging.AddConsole();
    })
    .UseConsoleLifetime();

var host = builder.Build();

using var scope = host.Services.CreateScope();
var myService = scope.ServiceProvider.GetRequiredService<MyService>();
await myService.RunAsync();

await host.RunAsync();

public class BlobStorageLogger : ILogger
{
    private readonly string _categoryName;
    private readonly AzureBlobLoggerOptions _options;

    public BlobStorageLogger(string categoryName, AzureBlobLoggerOptions options)
    {
        _categoryName = categoryName;
        _options = options;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.Filter.MinLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);

        // Write to Azure Blob Storage
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(_options.ContainerName);
        container.CreateIfNotExists();
        CloudBlockBlob blob = container.GetBlockBlobReference($"{_categoryName}/{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log");
        blob.UploadTextAsync(message).Wait();

        // Write to console
        Console.WriteLine($"{DateTime.UtcNow.ToString("o")} [{logLevel}] {_categoryName}: {message}");
    }
}
