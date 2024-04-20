using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Developer
{
    public Guid Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public Guid DeveloperAccessLevelId { get; set; }

    public bool IsOnline { get; set; }

    public virtual DeveloperAccessLevel DeveloperAccessLevel { get; set; } = null!;
}
