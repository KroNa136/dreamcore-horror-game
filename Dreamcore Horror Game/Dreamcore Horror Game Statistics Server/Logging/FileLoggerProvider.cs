namespace DreamcoreHorrorGameStatisticsServer.Logging;

public class FileLoggerProvider(string filePathTemplate, long maxFileSize) : ILoggerProvider
{
    private readonly string _filePathTemplate = filePathTemplate;
    private readonly long _maxFileSize = maxFileSize;

    public ILogger CreateLogger(string categoryName)
        => new FileLogger(_filePathTemplate, _maxFileSize, categoryName);

    public void Dispose() => GC.SuppressFinalize(this);
}
