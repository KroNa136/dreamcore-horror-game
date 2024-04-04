using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server.Models.Database;

public partial class Developer
{
    public Guid Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid DeveloperAccessLevelId { get; set; }

    public virtual DeveloperAccessLevel DeveloperAccessLevel { get; set; } = null!;
}
