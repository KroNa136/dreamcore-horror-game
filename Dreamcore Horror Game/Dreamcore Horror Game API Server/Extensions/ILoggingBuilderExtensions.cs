﻿using DreamcoreHorrorGameApiServer.Logging;

namespace DreamcoreHorrorGameApiServer.Extensions
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePathTemplate, long maxFileSize)
            => builder.AddProvider(new FileLoggerProvider(filePathTemplate, maxFileSize));
    }
}
