using Quartz;
using Simulation.Services;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IMotionCalculator, MotionCalculator>();
builder.Services.AddSingleton<ISpeedController, SpeedCalculator>();
builder.Services.AddSingleton<IOrientationCalculator, OrientationCalculator>();
builder.Services.AddTransient<FlightPathService>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<FlightPathUpdateJob>(opts => opts.WithIdentity("FlightPathUpdateJob"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddSingleton<UAVManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
