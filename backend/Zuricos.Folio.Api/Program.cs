using Zuricos.Folio.Api.Setup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.SetupConfig();
builder.SetupServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

// Map health check endpoints
app.MapHealthChecks("/healthz");
app.MapHealthChecks(
  "/healthz/ready",
  new() { Predicate = healthCheck => healthCheck.Tags.Contains("ready") }
);
app.MapHealthChecks(
  "/healthz/live",
  new() { Predicate = healthCheck => healthCheck.Tags.Contains("live") }
);

app.Run();
