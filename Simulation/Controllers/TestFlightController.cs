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

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateFlightPath([FromBody] FlightPathRequest request)
    {
        var telemetry = new Dictionary<TelemetryFields, double>
        {
            [TelemetryFields.LocationLatitude] = request.StartLocation.Latitude,
            [TelemetryFields.LocationLongitude] = request.StartLocation.Longitude,
            [TelemetryFields.LocationAltitudeAmsl] = request.StartLocation.Altitude,
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

        var flightService = new FlightPathService(
            uav,
            request.DestinationLocation,          
            request.CruiseAltitude ?? 400.0,    
            new MotionCalculator(),
            new SpeedController(),
            new OrientationCalculator(),
            _logger);

        var tcs = new TaskCompletionSource<bool>();
        flightService.MissionCompleted += () => tcs.SetResult(true);
        flightService.StartFlightPath();
        await tcs.Task;
        flightService.Dispose();

        return Ok(new FlightPathResponse
        {
            Message = "Simulation completed successfully",
            StartLocation = request.StartLocation,
            DestinationLocation = request.DestinationLocation,
            CruiseAltitude = request.CruiseAltitude ?? 400.0,
            UavId = uav.Id
        });
    }

    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        var request = new FlightPathRequest
        {
            StartLocation = new Location(40.6413, -73.7781, 10.0),
            DestinationLocation = new Location(40.6460, -73.7790, 100.0),
            CruiseAltitude = 200.0
        };

        return await CalculateFlightPath(request);
    }
}

public class FlightPathRequest
{
    public Location StartLocation { get; set; }
    public Location DestinationLocation { get; set; }
    public double? CruiseAltitude { get; set; }
}

public class FlightPathResponse
{
    public string Message { get; set; } = string.Empty;
    public Location StartLocation { get; set; }
    public Location DestinationLocation { get; set; }
    public double CruiseAltitude { get; set; }
    public string UavId { get; set; } = string.Empty;
}
