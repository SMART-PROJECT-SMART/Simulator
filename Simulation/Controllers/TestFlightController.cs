using Microsoft.AspNetCore.Mvc;
using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

namespace Simulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestFlightController : ControllerBase
{
    private readonly ILogger<FlightPathService> _logger;

    public TestFlightController(ILogger<FlightPathService> logger)
    {
        _logger = logger;
    }

    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        var start = new Location(40.6413, -73.7781, 10.0);
        var touchdown = new Location(40.6460, -73.7790, 10.0);

        double cruiseAltitude = 400.0;

        var telemetry = new Dictionary<TelemetryFields, double>
        {
            [TelemetryFields.LocationLatitude] = start.Latitude,
            [TelemetryFields.LocationLongitude] = start.Longitude,
            [TelemetryFields.LocationAltitudeAmsl] = start.Altitude,
            [TelemetryFields.LocationGroundSpeed] = 0.0,
            [TelemetryFields.LocationYaw] = 0.0,
            [TelemetryFields.LocationPitch] = 0.0,
            [TelemetryFields.LocationRoll] = 0.0
        };

        var uav = new UAV
        {
            Id = "UAV-001",
            TelemetryData = telemetry,
            UAVType = UAVTypes.A1,
            WingId = 1,
            CurrentMissionId = "M-001",
            MaxAcceleration = 5.0,
            MaxDeceleration = 5.0,
            MaxCruiseSpeedKmph = 100.0
        };

        var motionCalculator = new MotionCalculator();
        var speedController = new SpeedController();
        var orientationCalculator = new OrientationCalculator();

        var flightService = new FlightPathService(
            uav,
            touchdown,          
            cruiseAltitude,    
            motionCalculator,
            speedController,
            orientationCalculator,
            _logger);

        var tcs = new TaskCompletionSource<bool>();
        flightService.MissionCompleted += () => tcs.SetResult(true);

        flightService.StartFlightPath();
        await tcs.Task;

        return Ok("Simulation run complete. Check logs for Climb→Cruise→Descent.");
    }

}
