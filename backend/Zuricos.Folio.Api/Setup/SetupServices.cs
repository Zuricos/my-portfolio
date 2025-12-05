using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Services;
using Zuricos.Folio.Data;

namespace Zuricos.Folio.Api.Setup;

public static class HostApplicationBuilderServiceExtension
{
  /// <summary>
  /// Setup the services for the application
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  public static object SetupServices(this IHostApplicationBuilder builder)
  {
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IPortfolioService, PortfolioService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IActivityService, ActivityService>();
    builder.Services.AddScoped<IAssetCatalogService, AssetCatalogService>();

    string provider = builder.Configuration.GetValue("DatabaseProvider", "psql");
    string connectionString = provider switch
    {
      "psql" => builder.Configuration.GetConnectionString("psql")
        ?? throw new InvalidOperationException("PostgreSQL connection string 'psql' is required."),
      _ => throw new NotSupportedException($"Database provider '{provider}' is not supported."),
    };

    builder.Services.AddDbContextFactory<FolioDbContext>(options =>
    {
      _ = provider switch
      {
        "psql" => options.UseNpgsql(
          connectionString,
          x => x.MigrationsAssembly("Zuricos.Folio.Migrations.Psql")
        ),
        _ => throw new NotSupportedException($"Database provider '{provider}' is not supported."),
      };
    });

    // Add health checks with appropriate tags for readiness and liveness
    builder
      .Services.AddHealthChecks()
      .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"), ["live"])
      .AddDbContextCheck<FolioDbContext>("database", tags: ["ready", "database"])
      .AddNpgSql(connectionString, name: "postgresql", tags: ["ready", "database"]);

    string allowHost = builder.Configuration.GetValue("AllowedHosts", "*");
    builder.Services.AddCors(options =>
    {
      options.AddPolicy(
        "AllowHost",
        builder => builder.WithOrigins(allowHost).AllowAnyMethod().AllowAnyHeader()
      );
    });
    return builder;
  }
}
