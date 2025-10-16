using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data;

public class FolioDbContext(DbContextOptions<FolioDbContext> options) : DbContext(options)
{
  public DbSet<Portfolio> Portfolios { get; set; } = null!;
  public DbSet<Account> Accounts { get; set; } = null!;
  public DbSet<Asset> Assets { get; set; } = null!;
  public DbSet<Activity> Activities { get; set; } = null!;
  public DbSet<AssetHistory> AssetHistories { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Portfolio>(builder => builder.ConfigurePortfolios());
    modelBuilder.Entity<Account>(builder => builder.ConfigureAccounts());
    modelBuilder.Entity<Asset>(builder => builder.ConfigureAssets());
    modelBuilder.Entity<Activity>(builder => builder.ConfigureActivities());
    modelBuilder.Entity<AssetHistory>(builder => builder.ConfigureAssetHistories());
    modelBuilder.Entity<User>(builder => builder.ConfigureUserSettings());
    base.OnModelCreating(modelBuilder);
  }
}

file static class ModelBuilderExtensions
{
  public static void ConfigurePortfolios(this EntityTypeBuilder<Portfolio> builder)
  {
    builder.HasKey(p => p.Id);
    builder.Property(p => p.UserId).IsRequired();
    builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
    builder.Property(p => p.Institution).HasMaxLength(120);
    builder.Property(p => p.DisplayCurrency).IsRequired().HasMaxLength(3);
    builder.Property(p => p.Notes).HasMaxLength(500);
    builder.Property(p => p.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(p => p.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(p => p.DeletedUtc);

    builder.HasIndex(p => new { p.UserId, p.IsDeleted });
  }

  public static void ConfigureAccounts(this EntityTypeBuilder<Account> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.UserId).IsRequired();
    builder.Property(a => a.PortfolioId).IsRequired();
    builder.Property(a => a.Name).IsRequired().HasMaxLength(120);
    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.DisplayCurrency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.Category).HasMaxLength(80);
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    builder
      .HasOne(a => a.Portfolio)
      .WithMany(p => p.Accounts)
      .HasForeignKey(a => a.PortfolioId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasIndex(a => new
    {
      a.UserId,
      a.PortfolioId,
      a.IsDeleted,
    });
  }

  public static void ConfigureAssets(this EntityTypeBuilder<Asset> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.UserId).IsRequired();
    builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
    builder.Property(a => a.Isin).IsRequired().HasMaxLength(12);
    builder.Property(a => a.Symbol).IsRequired().HasMaxLength(15);
    builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.DataSource).IsRequired();
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);

    builder.HasIndex(a => new { a.UserId, a.IsDeleted });
    builder.HasIndex(a => a.Symbol).IsUnique();
  }

  public static void ConfigureActivities(this EntityTypeBuilder<Activity> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.UserId).IsRequired();
    builder
      .HasOne(a => a.Account)
      .WithMany(a => a.Activities)
      .HasForeignKey(a => a.AccountId)
      .OnDelete(DeleteBehavior.Cascade);
    builder
      .HasOne(a => a.Asset)
      .WithMany()
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.Quantity).HasColumnType(ConstValues.QuantityColumnType);
    builder.Property(a => a.Amount).HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.BookCurrency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.CounterAmount).HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.CounterCurrency).HasMaxLength(3);
    builder.Property(a => a.FxRate).HasColumnType(ConstValues.FxRateColumnType);
    builder.Property(a => a.SettledAmount).HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.UnitPrice).HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.UnitPriceCurrency).HasMaxLength(3);
    builder.Property(a => a.Tax).HasColumnType(ConstValues.ChargeColumnType);
    builder.Property(a => a.Fees).HasColumnType(ConstValues.ChargeColumnType);
    builder.Property(a => a.FeesCurrency).HasMaxLength(3);
    builder.Property(a => a.Description).HasMaxLength(500);
    builder.Property(a => a.TransferGroupId);
    builder.Property(a => a.OccurredOn).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.DeletedUtc);
  }

  public static void ConfigureAssetHistories(this EntityTypeBuilder<AssetHistory> builder)
  {
    builder.HasKey(a => a.Id);
    builder
      .HasOne(a => a.Asset)
      .WithMany(a => a.History)
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.Property(a => a.CreatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedUtc).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.Date).IsRequired();
    builder.Property(a => a.Open).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.High).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Low).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Close).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.AdjustedClose).IsRequired().HasColumnType(ConstValues.MoneyColumnType);
    builder.Property(a => a.Volume).IsRequired().HasColumnType(ConstValues.QuantityColumnType);
    builder.Property(a => a.DeletedUtc);
  }

  public static void ConfigureUserSettings(this EntityTypeBuilder<User> builder)
  {
    builder.HasKey(a => a.Id);

    builder
      .HasMany(a => a.Portfolios)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder
      .HasMany(a => a.Accounts)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder
      .HasMany(a => a.Activities)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.Property(a => a.Language).IsRequired().HasMaxLength(5);
    builder.Property(a => a.Theme).IsRequired();
    builder.Property(a => a.BaseCurrency).IsRequired().HasMaxLength(3);
  }
}
