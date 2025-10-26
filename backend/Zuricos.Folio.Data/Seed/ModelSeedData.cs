using Microsoft.EntityFrameworkCore;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Data.Seed;

/// <summary>
/// Contains seed data configuration for Entity Framework Core models.
/// Provides both essential reference data and optional development data.
/// </summary>
public static class ModelSeedData
{
  /// <summary>
  /// Seeds essential reference data required for the application to function.
  /// This data is always applied during model creation.
  /// </summary>
  public static void SeedReferenceData(ModelBuilder modelBuilder)
  {
    // Seed default user for single-tenant mode using anonymous object
    modelBuilder
      .Entity<User>()
      .HasData(
        new
        {
          Id = ConstValues.DefaultUserId,
          Language = "en-US",
          Theme = Theme.System,
          BaseCurrency = "USD",
        }
      );
  }

  /// <summary>
  /// Seeds development data for local testing.
  /// Call this method conditionally based on environment or configuration.
  /// </summary>
  public static void SeedDevelopmentData(ModelBuilder modelBuilder)
  {
    var portfolioId = new Guid("11111111-1111-1111-1111-111111111111");
    var cashAccountId = new Guid("22222222-2222-2222-2222-222222222222");
    var securitiesAccountId = new Guid("33333333-3333-3333-3333-333333333333");
    var assetId = new Guid("44444444-4444-4444-4444-444444444444");
    var baseTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    SeedSamplePortfolio(modelBuilder, portfolioId, baseTime);
    SeedSampleAccounts(modelBuilder, portfolioId, cashAccountId, securitiesAccountId, baseTime);
    SeedSampleAsset(modelBuilder, assetId, baseTime);
    SeedSampleActivities(modelBuilder, cashAccountId, securitiesAccountId, assetId, baseTime);
  }

  private static void SeedSamplePortfolio(
    ModelBuilder modelBuilder,
    Guid portfolioId,
    DateTimeOffset baseTime
  )
  {
    modelBuilder
      .Entity<Portfolio>()
      .HasData(
        new
        {
          Id = portfolioId,
          UserId = ConstValues.DefaultUserId,
          Name = "Sample Brokerage Account",
          Institution = "Interactive Brokers",
          DisplayCurrency = "USD",
          Notes = "Development sample portfolio",
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        }
      );
  }

  private static void SeedSampleAccounts(
    ModelBuilder modelBuilder,
    Guid portfolioId,
    Guid cashAccountId,
    Guid securitiesAccountId,
    DateTimeOffset baseTime
  )
  {
    modelBuilder
      .Entity<Account>()
      .HasData(
        new
        {
          Id = cashAccountId,
          UserId = ConstValues.DefaultUserId,
          PortfolioId = portfolioId,
          Name = "USD Cash",
          Type = AccountType.Cash,
          DisplayCurrency = "USD",
          Category = "Cash Management",
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        },
        new
        {
          Id = securitiesAccountId,
          UserId = ConstValues.DefaultUserId,
          PortfolioId = portfolioId,
          Name = "Securities Trading",
          Type = AccountType.Securities,
          DisplayCurrency = "USD",
          Category = "Equity Trading",
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        }
      );
  }

  private static void SeedSampleAsset(
    ModelBuilder modelBuilder,
    Guid assetId,
    DateTimeOffset baseTime
  )
  {
    modelBuilder
      .Entity<Asset>()
      .HasData(
        new
        {
          Id = assetId,
          UserId = ConstValues.DefaultUserId,
          Name = "Apple Inc.",
          Symbol = "AAPL",
          Isin = "US0378331005",
          Currency = "USD",
          DataSource = DataSource.Yahoo,
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        }
      );
  }

  private static void SeedSampleActivities(
    ModelBuilder modelBuilder,
    Guid cashAccountId,
    Guid securitiesAccountId,
    Guid assetId,
    DateTimeOffset baseTime
  )
  {
    modelBuilder
      .Entity<Activity>()
      .HasData(
        new
        {
          Id = new Guid("55555555-5555-5555-5555-555555555555"),
          UserId = ConstValues.DefaultUserId,
          AccountId = cashAccountId,
          AssetId = (Guid?)null,
          Type = ActivityType.Deposit,
          Quantity = 0m,
          Amount = 10000m,
          BookCurrency = "USD",
          CounterAmount = (decimal?)null,
          CounterCurrency = (string?)null,
          FxRate = (decimal?)null,
          SettledAmount = (decimal?)null,
          UnitPrice = (decimal?)null,
          UnitPriceCurrency = (string?)null,
          Tax = (decimal?)null,
          Fees = (decimal?)null,
          FeesCurrency = (string?)null,
          Description = "Initial deposit",
          TransferGroupId = (Guid?)null,
          OccurredOn = baseTime.AddDays(-30),
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        },
        new
        {
          Id = new Guid("66666666-6666-6666-6666-666666666666"),
          UserId = ConstValues.DefaultUserId,
          AccountId = securitiesAccountId,
          AssetId = (Guid?)assetId,
          Type = ActivityType.Buy,
          Quantity = 50m,
          Amount = 8500m,
          BookCurrency = "USD",
          CounterAmount = (decimal?)null,
          CounterCurrency = (string?)null,
          FxRate = (decimal?)null,
          SettledAmount = (decimal?)null,
          UnitPrice = (decimal?)170m,
          UnitPriceCurrency = "USD",
          Tax = (decimal?)null,
          Fees = (decimal?)5m,
          FeesCurrency = "USD",
          Description = "Purchase AAPL shares",
          TransferGroupId = (Guid?)null,
          OccurredOn = baseTime.AddDays(-25),
          CreatedUtc = baseTime,
          UpdatedUtc = baseTime,
          IsDeleted = false,
          DeletedUtc = (DateTimeOffset?)null,
        }
      );
  }
}
