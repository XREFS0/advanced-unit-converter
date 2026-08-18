using Microsoft.Extensions.Logging;

namespace AdvancedUnitConverter.Infrastructure.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logFilePath;

        public FileLoggerProvider(string? logDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                logDirectory = Path.Combine(appData, "AdvancedUnitConverter", "Logs");
            }
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, $"app_{DateTime.UtcNow:yyyyMMdd}.log");
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _logFilePath);

        public void Dispose() { }
    }

    public class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;
        private static readonly object _lock = new();

        public FileLogger(string categoryName, string filePath)
        {
            _categoryName = categoryName;
            _filePath = filePath;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string message = formatter(state, exception);
            string logLine = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [{logLevel}] [{_categoryName}] {message}";
            if (exception != null)
            {
                logLine += Environment.NewLine + exception;
            }

            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_filePath, logLine + Environment.NewLine);
                }
                catch
                {
                    // Ignore disk logging failure to avoid crashing application
                }
            }
        }
    }
}
