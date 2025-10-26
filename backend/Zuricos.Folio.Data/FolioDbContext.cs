using Microsoft.EntityFrameworkCore;
using Zuricos.Folio.Data.Configuration;
using Zuricos.Folio.Data.Models;
using Zuricos.Folio.Data.Seed;

namespace Zuricos.Folio.Data;

/// <summary>
/// Entity Framework Core database context for the Folio portfolio application.
/// Provides access to portfolios, accounts, activities, assets, and related entities.
/// </summary>
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
    // Apply entity configurations
    modelBuilder.ApplyConfiguration(new PortfolioConfiguration());
    modelBuilder.ApplyConfiguration(new AccountConfiguration());
    modelBuilder.ApplyConfiguration(new AssetConfiguration());
    modelBuilder.ApplyConfiguration(new ActivityConfiguration());
    modelBuilder.ApplyConfiguration(new AssetHistoryConfiguration());
    modelBuilder.ApplyConfiguration(new UserConfiguration());

    // Seed reference data
    ModelSeedData.SeedReferenceData(modelBuilder);

    base.OnModelCreating(modelBuilder);
  }
}
