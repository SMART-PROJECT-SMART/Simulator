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
        if (telemetry == null)
            return BadRequest("TelemetryData is required on the UAV.");
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

        flightService.StartFlightPath();

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

        var uav = new Searcher(
            tailId: 1,
            startLocation: startLocation);

        uav.TelemetryData[TelemetryFields.YawDeg] = 270.0;

        var destination = new Location(40.6460, -73.7790, 100.0);

        var request = new SimulateDto(uav, destination);

        return await CalculateFlightPath(request);
    }

    [HttpGet("run-hermes450")]
    public async Task<IActionResult> RunHermes450()
    {
        var startLocation = new Location(40.6413, -73.7781, 10.0);

        var uav = new Hermes450(
            tailId: 2,
            startLocation: startLocation);

        var destination = new Location(40.6460, -73.7790, 100.0);

        var request = new SimulateDto(uav, destination);

        return await CalculateFlightPath(request);
    }
}
