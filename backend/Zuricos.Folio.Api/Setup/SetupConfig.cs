namespace Zuricos.Folio.Api.Setup;
public static class HostApplicationBuilderConfigExtension
{

  /// <summary>
  /// Setup the configuration for the application
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  public static IHostApplicationBuilder SetupConfig(this IHostApplicationBuilder builder)
  {
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
    return builder;
  }
}