using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the Asset entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
  public void Configure(EntityTypeBuilder<Asset> builder)
  {
    builder.HasKey(a => a.Id);

    // Required fields with constraints
    builder.Property(a => a.UserId).IsRequired();
    builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
    builder.Property(a => a.Isin).IsRequired().HasMaxLength(12);
    builder.Property(a => a.Symbol).IsRequired().HasMaxLength(15);
    builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.DataSource).IsRequired();

    // Audit timestamps
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    // Performance indexes and unique constraints
    builder
      .HasIndex(a => new { a.UserId, a.IsDeleted })
      .HasDatabaseName("IX_Assets_UserId_IsDeleted");
    builder.HasIndex(a => a.Symbol).IsUnique().HasDatabaseName("IX_Assets_Symbol_Unique");
    builder
      .HasIndex(a => new { a.Symbol, a.Currency })
      .HasDatabaseName("IX_Assets_Symbol_Currency");
    builder.HasIndex(a => a.DataSource).HasDatabaseName("IX_Assets_DataSource");
    builder
      .HasIndex(a => new { a.Currency, a.IsDeleted })
      .HasDatabaseName("IX_Assets_Currency_IsDeleted");

    // Check constraints for data integrity
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_Assets_Currency_Format",
        "LENGTH(\"Currency\") = 3 AND \"Currency\" = UPPER(\"Currency\")"
      );
      t.HasCheckConstraint("CK_Assets_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
      t.HasCheckConstraint("CK_Assets_Symbol_NotEmpty", "LENGTH(TRIM(\"Symbol\")) > 0");
      t.HasCheckConstraint(
        "CK_Assets_ISIN_Format",
        "LENGTH(\"Isin\") <= 12 AND \"Isin\" ~ '^[A-Z]{2}[A-Z0-9]{9}[0-9]{1}$'"
      ); // Basic ISIN format check
      t.HasCheckConstraint(
        "CK_Assets_SoftDelete_Integrity",
        "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
      );
    });
  }
}
