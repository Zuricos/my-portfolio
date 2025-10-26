using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Configuration;

/// <summary>
/// Entity Framework Core configuration for the Account entity.
/// Defines table mapping, constraints, indexes, and relationships.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
  public void Configure(EntityTypeBuilder<Account> builder)
  {
    builder.HasKey(a => a.Id);

    // Required fields with constraints
    builder.Property(a => a.UserId).IsRequired();
    builder.Property(a => a.PortfolioId).IsRequired();
    builder.Property(a => a.Name).IsRequired().HasMaxLength(120);
    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.DisplayCurrency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.Category).HasMaxLength(80);

    // Audit timestamps
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    // Relationships with explicit cascade behavior
    builder
      .HasOne(a => a.Portfolio)
      .WithMany(p => p.Accounts)
      .HasForeignKey(a => a.PortfolioId)
      .OnDelete(DeleteBehavior.Cascade); // Cascade when portfolio is deleted

    // Performance indexes
    builder
      .HasIndex(a => new
      {
        a.UserId,
        a.PortfolioId,
        a.IsDeleted,
      })
      .HasDatabaseName("IX_Accounts_UserId_PortfolioId_IsDeleted");
    builder
      .HasIndex(a => new
      {
        a.PortfolioId,
        a.Type,
        a.IsDeleted,
      })
      .HasDatabaseName("IX_Accounts_PortfolioId_Type_IsDeleted");
    builder.HasIndex(a => a.Name).HasDatabaseName("IX_Accounts_Name");

    // Check constraints for data integrity
    builder.ToTable(t =>
    {
      t.HasCheckConstraint(
        "CK_Accounts_DisplayCurrency_Format",
        "LENGTH(\"DisplayCurrency\") = 3 AND \"DisplayCurrency\" = UPPER(\"DisplayCurrency\")"
      );
      t.HasCheckConstraint("CK_Accounts_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
      t.HasCheckConstraint("CK_Accounts_Type_Valid", "\"Type\" IN (0, 1, 2)"); // Cash, Securities, Cryptocurrency
      t.HasCheckConstraint(
        "CK_Accounts_SoftDelete_Integrity",
        "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
      );
    });
  }
}
