using DreamcoreHorrorGameApiServer.Logging;

namespace DreamcoreHorrorGameApiServer.Extensions
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath, long maxFileSize)
            => builder.AddProvider(new FileLoggerProvider(filePath, maxFileSize));
    }
}
