using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.UAVs;
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

    public SimulationController(ILogger<FlightPathService> logger)
    {
        _logger = logger;
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
    {
        var flightService = new FlightPathService(
            dto.UAV,
            dto.Destination,
            dto.UAV.CruiseAltitude,
            new MotionCalculator(),
            new SpeedController(),
            new OrientationCalculator(),
            _logger);

        var tcs = new TaskCompletionSource<bool>();
        flightService.MissionCompleted += () => tcs.SetResult(true);
        flightService.StartFlightPath();
        await tcs.Task;
        flightService.Dispose();

        return Ok();
    }

    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
       
        Location startLocation = new Location(40.6413, -73.7781, 10.0);
        var uav = new Searcher(1, startLocation);

        var destination = new Location(40.6460, -73.7790, 100.0);

        var request = new SimulateDto
        {
            UAV = uav,
            Destination = destination
        };

        return await CalculateFlightPath(request);
    }
}
