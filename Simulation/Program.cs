using Shared.Services;
using Simulation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.AddFlightPathCalculators();
builder.Services.AddFlightPathServices();
builder.Services.AddQuartzServices();
builder.Services.AddManagementServices();
builder.Services.AddSharedServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.Run();