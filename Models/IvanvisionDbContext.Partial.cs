using Microsoft.EntityFrameworkCore;

namespace WebWerverPart.Models;

public partial class IvanvisionDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserRole)
                .HasColumnName("user_role")
                .HasColumnType("user_status");
        });
    }
}