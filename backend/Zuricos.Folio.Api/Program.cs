using Zuricos.Folio.Api.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.SetupConfig();
builder.SetupServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.Run();
