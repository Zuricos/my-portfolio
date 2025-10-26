using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the Portfolio entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
  public void Configure(EntityTypeBuilder<Portfolio> builder)
  {
    builder.HasKey(p => p.Id);

    // Required fields with constraints
    builder.Property(p => p.UserId).IsRequired();
    builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
    builder.Property(p => p.Institution).HasMaxLength(120);
    builder.Property(p => p.DisplayCurrency).IsRequired().HasMaxLength(3);
    builder.Property(p => p.Notes).HasMaxLength(500);

    // Audit timestamps
    builder.Property(p => p.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(p => p.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(p => p.DeletedUtc);

    // Performance indexes
    builder
      .HasIndex(p => new { p.UserId, p.IsDeleted })
      .HasDatabaseName("IX_Portfolios_UserId_IsDeleted");
    builder.HasIndex(p => p.Name).HasDatabaseName("IX_Portfolios_Name");
    builder
      .HasIndex(p => new
      {
        p.UserId,
        p.Institution,
        p.IsDeleted,
      })
      .HasDatabaseName("IX_Portfolios_UserId_Institution_IsDeleted");

    // Check constraints for data integrity using modern EF Core 9 syntax
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_Portfolios_DisplayCurrency_Format",
        "LENGTH(\"DisplayCurrency\") = 3 AND \"DisplayCurrency\" = UPPER(\"DisplayCurrency\")"
      );
      t.HasCheckConstraint("CK_Portfolios_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
      t.HasCheckConstraint(
        "CK_Portfolios_SoftDelete_Integrity",
        "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
      );
    });
  }
}
