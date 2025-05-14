
using Microsoft.EntityFrameworkCore;

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

    var provider = builder.Configuration.GetValue("DatabaseProvider", "psql");
    builder.Services.AddDbContextFactory<FolioDbContext>(options =>
    {
      _ = provider switch
      {
        "psql" => options.UseNpgsql(
          builder.Configuration.GetConnectionString("psql"),
          x => x.MigrationsAssembly("Zuricos.Folio.Migrations.Psql")),
        "sqlite" => options.UseSqlite(
          builder.Configuration.GetConnectionString("sqlite"),
          x => x.MigrationsAssembly("Zuricos.Folio.Migrations.Sqlite")),
        _ => throw new NotSupportedException($"Database provider '{provider}' is not supported.")
      };
    });


    string allowHost = builder.Configuration.GetValue("AllowedHosts", "*");
    builder.Services.AddCors(options =>
    {
      options.AddPolicy("AllowHost", builder =>
            builder
              .WithOrigins(allowHost)
              .AllowAnyMethod()
              .AllowAnyHeader());
    });
    return builder;
  }
}
