using Zuricos.Folio.Api.Setup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.SetupConfig();
builder.SetupServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.Run();
