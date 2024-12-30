namespace DreamcoreHorrorGameApiServer.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePathTemplate;
    private readonly long _maxFileSize;

    public FileLoggerProvider(string filePathTemplate, long maxFileSize)
    {
        _filePathTemplate = filePathTemplate;
        _maxFileSize = maxFileSize;
    }

    public ILogger CreateLogger(string categoryName)
        => new FileLogger(_filePathTemplate, _maxFileSize, categoryName);

    public void Dispose() => GC.SuppressFinalize(this);
}
