using System;
using System.Collections.Generic;

namespace WebWerverPart.Models;

public partial class BugReport
{
    public int IdReport { get; set; }

    public int UserId { get; set; }

    public string ReportDescription { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
