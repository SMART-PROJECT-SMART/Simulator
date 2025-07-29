using Microsoft.AspNetCore.Mvc;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

namespace Simulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ILogger<FlightPathService> _logger;
    private readonly IMotionCalculator _motionCalculator;
    private readonly ISpeedController _speedController;
    private readonly IOrientationCalculator _orientationCalculator;

    public SimulationController(
        ILogger<FlightPathService> logger,
        IMotionCalculator motionCalculator,
        ISpeedController speedController,
        IOrientationCalculator orientationCalculator)
    {
        _logger = logger;
        _motionCalculator = motionCalculator;
        _speedController = speedController;
        _orientationCalculator = orientationCalculator;
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
    {
        var telemetry = dto.UAV.TelemetryData;
        double cruiseAlt = telemetry.GetValueOrDefault(
            TelemetryFields.CruiseAltitude,
            dto.Destination.Altitude);

        var flightService = new FlightPathService(
            dto.UAV,
            dto.Destination,
            cruiseAlt,
            _motionCalculator,
            _speedController,
            _orientationCalculator,
            _logger);

        var tcs = new TaskCompletionSource<bool>();
        flightService.MissionCompleted += () => tcs.SetResult(true);

        await tcs.Task;
        flightService.Dispose();

        return Ok();
    }

    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        var startLocation = new Location(40.6413, -73.7781, 10.0);

        var initialTelemetry = new Dictionary<TelemetryFields, double>
        {
            [TelemetryFields.Mass] = 73500 + (23860.0 / 1.25),
            [TelemetryFields.FrontalSurface] = 12.6,
            [TelemetryFields.WingsSurface] = 122.6,
            [TelemetryFields.DragCoefficient] = 0.095,
            [TelemetryFields.LiftCoefficient] = 0.9,
            [TelemetryFields.ThrustMax] = 120000 * 2,
            [TelemetryFields.MaxCruiseSpeedKmph] = 250.0,
            [TelemetryFields.MaxAccelerationMps2] = 3.0,
            [TelemetryFields.MaxDecelerationMps2] = -2.0,
            [TelemetryFields.CruiseAltitude] = 100.0,
            [TelemetryFields.CurrentSpeedKmph] = SimulationConstants.FlightPath.MIN_SPEED_KMH,
            [TelemetryFields.Latitude] = startLocation.Latitude,
            [TelemetryFields.longitude] = startLocation.Longitude,
            [TelemetryFields.Altitude] = startLocation.Altitude
        };

        var uav = new Searcher(
            tailId: 1,
            startLocation: startLocation,
            initialTelemetry: initialTelemetry);

        var destination = new Location(40.6460, -73.7790, 100.0);

        var request = new SimulateDto(uav, destination);

        return await CalculateFlightPath(request);
    }
}
