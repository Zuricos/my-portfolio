
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
  public static IHostApplicationBuilder SetupServices(this IHostApplicationBuilder builder)
  {
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddDbContextFactory<FolioDbContext>(options =>
    {
      options.UseNpgsql(builder.Configuration.GetConnectionString("FolioDb"));
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
