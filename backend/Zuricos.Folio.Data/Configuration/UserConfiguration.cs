using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the User entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(a => a.Id);

    // User-owned entity relationships
    builder
      .HasMany(a => a.Portfolios)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.Cascade); // Delete all portfolios when user is deleted

    builder
      .HasMany(a => a.Accounts)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade); // Let application handle account deletion

    builder
      .HasMany(a => a.Activities)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade); // Let application handle activity deletion

    // User preferences with constraints
    builder.Property(a => a.Language).IsRequired().HasMaxLength(5);
    builder.Property(a => a.Theme).IsRequired();
    builder.Property(a => a.BaseCurrency).IsRequired().HasMaxLength(3);

    // Performance indexes
    builder.HasIndex(a => a.BaseCurrency).HasDatabaseName("IX_Users_BaseCurrency");

    // Check constraints for data integrity
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_Users_BaseCurrency_Format",
        "LENGTH(\"BaseCurrency\") = 3 AND \"BaseCurrency\" = UPPER(\"BaseCurrency\")"
      );
      t.HasCheckConstraint("CK_Users_Language_Format", "\"Language\" ~ '^[a-z]{2}(-[A-Z]{2})?$'"); // ISO 639-1 format (e.g., 'en', 'en-US')
      t.HasCheckConstraint("CK_Users_Theme_Valid", "\"Theme\" IN (0, 1, 2)"); // System, Light, Dark enum values
    });
  }
}
