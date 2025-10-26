using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the Activity entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
  public void Configure(EntityTypeBuilder<Activity> builder)
  {
    builder.HasKey(a => a.Id);

    // Required foreign keys
    builder.Property(a => a.UserId).IsRequired();

    // Relationships with explicit cascade behavior
    builder
      .HasOne(a => a.Account)
      .WithMany(a => a.Activities)
      .HasForeignKey(a => a.AccountId)
      .OnDelete(DeleteBehavior.Cascade); // Cascade when account is deleted
    builder
      .HasOne(a => a.Asset)
      .WithMany()
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade); // Let application handle asset deletion

    // Core activity properties with precision
    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.Quantity).HasColumnType(ConstValues.QuantityColumnType);
    builder.Property(a => a.Amount).HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);

    // Multi-currency support (simplified)
    builder.Property(a => a.FxRate).HasColumnType(ConstValues.FxRateColumnType);
    builder.Property(a => a.SourceCurrency).HasMaxLength(3);

    // Charges (in same currency as Amount)
    builder.Property(a => a.Tax).HasColumnType(ConstValues.ChargeColumnType);
    builder.Property(a => a.Fees).HasColumnType(ConstValues.ChargeColumnType);

    // Metadata
    builder.Property(a => a.Description).HasMaxLength(500);
    builder.Property(a => a.TransferGroupId);

    // Timestamps
    builder.Property(a => a.OccurredOn).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    // Ignore calculated properties (not stored in database)
    builder.Ignore(a => a.UnitPrice);
    builder.Ignore(a => a.SourceAmount);

    // Performance indexes for common query patterns
    builder
      .HasIndex(a => new
      {
        a.AccountId,
        a.OccurredOn,
        a.IsDeleted,
      })
      .HasDatabaseName("IX_Activities_AccountId_OccurredOn_IsDeleted");
    builder
      .HasIndex(a => new
      {
        a.UserId,
        a.Type,
        a.OccurredOn,
      })
      .HasDatabaseName("IX_Activities_UserId_Type_OccurredOn");
    builder
      .HasIndex(a => new { a.AssetId, a.OccurredOn })
      .HasDatabaseName("IX_Activities_AssetId_OccurredOn")
      .HasFilter("\"AssetId\" IS NOT NULL"); // Partial index for asset activities only
    builder
      .HasIndex(a => a.TransferGroupId)
      .HasDatabaseName("IX_Activities_TransferGroupId")
      .HasFilter("\"TransferGroupId\" IS NOT NULL"); // Partial index for transfers only
    builder.HasIndex(a => a.OccurredOn).HasDatabaseName("IX_Activities_OccurredOn"); // Time-series queries

    // Check constraints for business rules and data integrity
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_Activities_Currency_Format",
        "LENGTH(\"Currency\") = 3 AND \"Currency\" = UPPER(\"Currency\")"
      );
      t.HasCheckConstraint(
        "CK_Activities_SourceCurrency_Format",
        "\"SourceCurrency\" IS NULL OR (LENGTH(\"SourceCurrency\") = 3 AND \"SourceCurrency\" = UPPER(\"SourceCurrency\"))"
      );
      t.HasCheckConstraint(
        "CK_Activities_Type_Valid",
        "\"Type\" IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 99)"
      ); // ActivityType enum values
      t.HasCheckConstraint("CK_Activities_Amount_Positive", "\"Amount\" > 0");
      t.HasCheckConstraint("CK_Activities_Quantity_NonNegative", "\"Quantity\" >= 0");
      t.HasCheckConstraint("CK_Activities_FxRate_Positive", "\"FxRate\" IS NULL OR \"FxRate\" > 0");
      t.HasCheckConstraint("CK_Activities_Fees_NonNegative", "\"Fees\" IS NULL OR \"Fees\" >= 0");
      t.HasCheckConstraint("CK_Activities_Tax_NonNegative", "\"Tax\" IS NULL OR \"Tax\" >= 0");
      t.HasCheckConstraint(
        "CK_Activities_SoftDelete_Integrity",
        "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
      );
      // Asset requirement for certain activity types
      t.HasCheckConstraint(
        "CK_Activities_Asset_Required_For_Securities",
        "(\"Type\" IN (4, 5) AND \"AssetId\" IS NOT NULL) OR \"Type\" NOT IN (4, 5)"
      ); // Buy/Sell require AssetId
      // Multi-currency consistency (simplified)
      t.HasCheckConstraint(
        "CK_Activities_MultiCurrency_Consistency",
        "(\"FxRate\" IS NULL AND \"SourceCurrency\" IS NULL) OR "
          + "(\"FxRate\" IS NOT NULL AND \"SourceCurrency\" IS NOT NULL)"
      );
    });
  }
}
