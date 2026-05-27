using System;
using System.Collections.Generic;

namespace WebWerverPart.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string UserLogin { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public virtual ICollection<BugReport> BugReports { get; set; } = new List<BugReport>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
