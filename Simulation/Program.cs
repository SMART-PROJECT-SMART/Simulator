using Quartz;
using Simulation.Common.constants;
using Simulation.Services;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.Quartz;
using Simulation.Services.UAVManager;
using Simulation.Configuration;
using Simulation.Services.PortManager;
using Simulation.Services.ICDDirectory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<ICDSettings>(builder.Configuration.GetSection(SimulationConstants.Config.ICD_DIRECTORY));
builder.Services.AddSingleton<IICDDirectory, ICDDirectory>();
builder.Services.AddSingleton<IMotionCalculator, MotionCalculator>();
builder.Services.AddSingleton<ISpeedController, SpeedCalculator>();
builder.Services.AddSingleton<IOrientationCalculator, OrientationCalculator>();
builder.Services.AddTransient<FlightPathService>();
builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<ISchedulerFactory>()
        .GetScheduler()
        .GetAwaiter()
        .GetResult()
);
builder.Services.AddSingleton<IQuartzManager, QuartzManager>();
builder.Services.AddSingleton<IUAVManager, UAVManager>();
builder.Services.AddSingleton<IPortManager, PortManager>();
builder.Services.AddSingleton<ChannelManager>();
var app = builder.Build();

var directory = app.Services.GetRequiredService<IICDDirectory>();
directory.LoadAllICDs();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.Run();