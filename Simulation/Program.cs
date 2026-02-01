using Core.Services;
using Simulation.Services;
using Simulation.Services.UAVStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddFlightPathCalculators();
builder.Services.AddFlightPathServices();
builder.Services.AddQuartzServices();
builder.Services.AddManagementServices();
builder.Services.AddDeviceManagerServices();
builder.Services.AddIcdDirectory();
builder.Services.AddSharedConfiguration(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.Run();