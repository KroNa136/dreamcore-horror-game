using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameStatisticsServer.Models;

public partial class Request
{
    public Guid Id { get; set; }

    public DateTime ReceptionTimestamp { get; set; }

    public Guid SenderId { get; set; }

    public Guid ControllerId { get; set; }

    public Guid MethodId { get; set; }

    public virtual Controller Controller { get; set; } = null!;

    public virtual Method Method { get; set; } = null!;

    public virtual Sender Sender { get; set; } = null!;
}
