using Simulation.Services;
using Simulation.Services.ICDDirectory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
builder.Services.AddIcdDirectoryServices();
builder.Services.AddFlightPathCalculators();
builder.Services.AddFlightPathServices();
builder.Services.AddQuartzServices();
builder.Services.AddManagementServices();

var app = builder.Build();

app.Services.GetRequiredService<IICDDirectory>().LoadAllICDs();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.Run();