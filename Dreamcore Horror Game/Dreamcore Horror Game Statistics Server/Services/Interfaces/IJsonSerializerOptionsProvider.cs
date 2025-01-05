﻿using System.Text.Json;

namespace DreamcoreHorrorGameStatisticsServer.Services;

public interface IJsonSerializerOptionsProvider
{
    public JsonSerializerOptions New { get; }
    public JsonSerializerOptions Default { get; }
}
