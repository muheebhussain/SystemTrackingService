using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureBlob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


public class CustomLoggerProvider : ILoggerProvider
{
    private readonly string _logDirectory;
    private readonly IDictionary<string, StreamWriter> _logFiles = new Dictionary<string, StreamWriter>();
    private readonly SemaphoreSlim _logFileLock = new SemaphoreSlim(1, 1);

    public CustomLoggerProvider(string logDirectory)
    {
        _logDirectory = logDirectory;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CustomLogger(this, categoryName);
    }

    public void Dispose()
    {
        foreach (var logFile in _logFiles.Values)
        {
            logFile.Dispose();
        }
        _logFiles.Clear();
    }

    internal async Task LogAsync(string serviceName, string processName, LogLevel logLevel, string message)
    {
        string logFileName = GetLogFileName(serviceName, processName);
        StreamWriter logFile = await GetLogFileAsync(logFileName);

        await _logFileLock.WaitAsync();
        try
        {
            await logFile.WriteLineAsync($"{DateTime.UtcNow.ToString("o")} [{logLevel}] {message}");
            await logFile.FlushAsync();
        }
        finally
        {
            _logFileLock.Release();
        }
    }

    private async Task<StreamWriter> GetLogFileAsync(string logFileName)
    {
        if (!_logFiles.TryGetValue(logFileName, out StreamWriter logFile))
        {
            await _logFileLock.WaitAsync();
            try
            {
                if (!_logFiles.TryGetValue(logFileName, out logFile))
                {
                    string logFilePath = Path.Combine(_logDirectory, logFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                    logFile = new StreamWriter(logFilePath, true);
                    _logFiles.Add(logFileName, logFile);
                }
            }
            finally
            {
                _logFileLock.Release();
            }
        }
        return logFile;
    }

    private string GetLogFileName(string serviceName, string processName)
    {
        string fileName = serviceName;
        if (!string.IsNullOrEmpty(processName))
        {
            fileName += "_" + processName;
        }
        return $"{fileName}/{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log";
    }
}
