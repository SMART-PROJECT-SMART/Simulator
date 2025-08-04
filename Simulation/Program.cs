using Quartz;
using Simulation.Services;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.Helpers;
using Simulation.Services.ICD;
using Simulation.Services.Quartz;
using Simulation.Services.UAVManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IMotionCalculator, MotionCalculator>();
builder.Services.AddSingleton<ISpeedController, SpeedCalculator>();
builder.Services.AddSingleton<IOrientationCalculator, OrientationCalculator>();
builder.Services.AddTransient<FlightPathService>();

builder.Services.AddQuartz();

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
    return schedulerFactory.GetScheduler().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IQuartzManager, QuartzManager>();
builder.Services.AddSingleton<IUAVManager, UAVManager>();
builder.Services.AddSingleton<IICDDirectory, ICDDirectory>();
builder.Services.AddSingleton<IICDNetworking, ICDNetworkingHelper>();
builder.Services.AddSingleton<StartUp>();
var app = builder.Build();

var startupService = app.Services.GetRequiredService<StartUp>();
startupService.LoadAndSendICDsOnStartup();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
