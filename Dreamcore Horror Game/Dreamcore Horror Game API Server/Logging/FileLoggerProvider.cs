namespace DreamcoreHorrorGameApiServer.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly long _maxFileSize;

    public FileLoggerProvider(string filePath, long maxFileSize)
    {
        _filePath = filePath;
        _maxFileSize = maxFileSize;
    }

    public ILogger CreateLogger(string categoryName)
        => new FileLogger(_filePath, _maxFileSize, categoryName);

    public void Dispose() => GC.SuppressFinalize(this);
}
