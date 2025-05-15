
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data;
public class FolioDbContext(DbContextOptions<FolioDbContext> options) : DbContext(options)
{
  public DbSet<Account> Accounts { get; set; } = null!;
  public DbSet<Asset> Assets { get; set; } = null!;
  public DbSet<Activity> Activities { get; set; } = null!;
  public DbSet<AssetHistory> AssetHistories { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Account>(builder => builder.ConfigureAccounts());
    modelBuilder.Entity<Asset>(builder => builder.ConfigureAssets());
    modelBuilder.Entity<Activity>(builder => builder.ConfigureActivities());
    modelBuilder.Entity<AssetHistory>(builder => builder.ConfigureAssetHistories());
    modelBuilder.Entity<User>(builder => builder.ConfigureUserSettings());
    base.OnModelCreating(modelBuilder);
  }
}

file static class ModelBuilderExtensions{
  public static void ConfigureAccounts(this EntityTypeBuilder<Account> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
  }

  public static void ConfigureAssets(this EntityTypeBuilder<Asset> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
    builder.Property(a => a.Isin).IsRequired().HasMaxLength(12);
    builder.Property(a => a.Symbol).IsRequired().HasMaxLength(5);
    builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.DataSource).IsRequired();
    builder.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
  }

  public static void ConfigureActivities(this EntityTypeBuilder<Activity> builder)
  {
    builder.HasKey(a => a.Id);
    builder.HasOne(a => a.Account)
      .WithMany(a => a.Activities)
      .HasForeignKey(a => a.AccountId)
      .OnDelete(DeleteBehavior.ClientCascade);
    builder.HasOne(a => a.Asset)
      .WithMany() // Do not specify a navigation property on Asset
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade);
    
    builder.Property(a => a.Type).IsRequired();
    builder.Property(a => a.Amount).IsRequired().HasColumnType(ConstValues.AmountPrecision);
    builder.Property(a => a.Price).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.PriceCurrency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.Tax).IsRequired().HasColumnType(ConstValues.FeePrecision);
    builder.Property(a => a.Fees).IsRequired().HasColumnType(ConstValues.FeePrecision);
    builder.Property(a => a.FeesCurrency).IsRequired().HasMaxLength(3);
    builder.Property(a => a.Description).HasMaxLength(500);
    builder.Property(a => a.TransactionId).IsRequired(false);
    builder.Property(a => a.ExchangeRate).IsRequired(false).HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
  }

  public static void ConfigureAssetHistories(this EntityTypeBuilder<AssetHistory> builder)
  {
    builder.HasKey(a => a.Id);
    builder.HasOne(a => a.Asset)
      .WithMany(a => a.History)
      .HasForeignKey(a => a.AssetId)
      .OnDelete(DeleteBehavior.ClientCascade);
    
    builder.Property(a => a.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(a => a.Date).IsRequired();
    builder.Property(a => a.Open).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.High).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.Low).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.Close).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
    builder.Property(a => a.Volume).IsRequired().HasColumnType(ConstValues.CurrenyPrecision);
  }

  public static void ConfigureUserSettings(this EntityTypeBuilder<User> builder)
  {
    builder.HasKey(a => a.Id);

    builder.HasMany(a => a.Accounts)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.HasMany(a => a.Activities)
      .WithOne()
      .HasForeignKey(a => a.UserId)
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.Property(a => a.Language).IsRequired().HasMaxLength(5);
    builder.Property(a => a.Theme).IsRequired();
    builder.Property(a => a.BaseCurrency).IsRequired().HasMaxLength(3);
  }
}