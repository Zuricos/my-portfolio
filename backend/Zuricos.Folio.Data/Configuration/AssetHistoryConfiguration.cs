using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the AssetHistory entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class AssetHistoryConfiguration : IEntityTypeConfiguration<AssetHistory>
{
  public void Configure(EntityTypeBuilder<AssetHistory> builder)
  {
    builder.HasKey(a => a.Id);

    // Relationship with asset
    builder
      .HasOne(a => a.Asset)
      .WithMany(a => a.History)
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade); // Let application handle asset deletion

    // Timestamps
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    // Historical data with proper precision
    builder.Property(a => a.Date).IsRequired();
    builder.Property(a => a.Open).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.High).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Low).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Close).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.AdjustedClose).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Volume).IsRequired().HasColumnType(ConstValues.QuantityColumnType);

    // Performance indexes for time-series queries
    builder
      .HasIndex(a => new
      {
        a.AssetId,
        a.Date,
        a.IsDeleted,
      })
      .IsUnique()
      .HasDatabaseName("IX_AssetHistories_AssetId_Date_Unique");
    builder
      .HasIndex(a => new { a.Date, a.IsDeleted })
      .HasDatabaseName("IX_AssetHistories_Date_IsDeleted");
    builder
      .HasIndex(a => new { a.AssetId, a.Date })
      .IsDescending(false, true) // Most recent first
      .HasDatabaseName("IX_AssetHistories_AssetId_Date_Desc");

    // Check constraints for OHLC data integrity
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_AssetHistories_OHLC_Positive",
        "\"Open\" > 0 AND \"High\" > 0 AND \"Low\" > 0 AND \"Close\" > 0 AND \"AdjustedClose\" > 0"
      );
      t.HasCheckConstraint("CK_AssetHistories_High_Low_Relationship", "\"High\" >= \"Low\"");
      t.HasCheckConstraint(
        "CK_AssetHistories_OHLC_Range",
        "\"Open\" BETWEEN \"Low\" AND \"High\" AND \"Close\" BETWEEN \"Low\" AND \"High\""
      );
      t.HasCheckConstraint("CK_AssetHistories_Volume_NonNegative", "\"Volume\" >= 0");
      t.HasCheckConstraint(
        "CK_AssetHistories_SoftDelete_Integrity",
        "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
      );
    });
  }
}
