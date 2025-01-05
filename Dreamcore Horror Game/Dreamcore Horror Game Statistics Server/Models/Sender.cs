using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameStatisticsServer.Models;

public partial class Sender
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
