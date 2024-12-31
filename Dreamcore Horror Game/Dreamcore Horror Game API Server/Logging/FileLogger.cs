using System.Security;
using System.Text;

namespace DreamcoreHorrorGameApiServer.Logging;

public class FileLogger(string filePathTemplate, long maxFileSize, string categoryName) : ILogger, IDisposable
{
    private readonly string _filePathTemplate = filePathTemplate;
    private readonly long _maxFileSize = maxFileSize;
    private readonly string _categoryName = categoryName;

    protected static readonly object s_lock = new();

    private string _filePath = string.Empty;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => this;
    public void Dispose() => GC.SuppressFinalize(this);

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        lock (s_lock)
        {
            StringBuilder sb = new();

            sb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff}{Environment.NewLine}");
            sb.Append($"{logLevel.ToString().ToUpper()} from {_categoryName} [{eventId}]{Environment.NewLine}");
            sb.Append($"{formatter(state, exception)}{Environment.NewLine}{Environment.NewLine}");

            _filePath = GetTodaysFilePath();

            try
            {
                if (!File.Exists(_filePath))
                    File.WriteAllText(_filePath, string.Empty);

                File.AppendAllText(_filePath, sb.ToString());
                LimitFileSize();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException)
            {
                var defaultForegroundColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}Failed to write a log message to the logging file: the file is inaccessible");
                Console.WriteLine(sb.ToString());
                Console.ForegroundColor = defaultForegroundColor;
            }
        }
    }

    private string GetTodaysFilePath() => string.Format(_filePathTemplate, DateTime.Now.ToString("yyyyMMdd"));

    private void LimitFileSize()
    {
        while (new FileInfo(_filePath).Length > _maxFileSize)
            RemoveFirstLogEntry();
    }

    private void RemoveFirstLogEntry()
    {
        List<string> lines = File.ReadAllLines(_filePath).ToList();

        while (!lines.ElementAt(0).Equals(string.Empty))
            lines.RemoveAt(0);

        lines.RemoveAt(0);

        File.WriteAllLines(_filePath, lines.ToArray());
    }
}
