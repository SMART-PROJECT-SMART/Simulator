using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simulation.Common.Enums;
using Simulation.Models.Mission;
using Simulation.Models.Uav;
using Simulation.Services;

namespace Simulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestFlightController(ILogger<FlightPathService> logger) : ControllerBase
{
    [HttpGet("run")]
    public async Task<IActionResult> Run()
    {
        var start = new Location(40.6413, -73.7781, 13.0);
        var dest = new Location(40.7306, -73.9352, 15.0);

        var telemetry = new Dictionary<TelemetryFields, double>
        {
            [TelemetryFields.LocationLatitude] = start.Latitude,
            [TelemetryFields.LocationLongitude] = start.Longitude,
            [TelemetryFields.LocationAltitudeAmsl] = start.Altitude,
            [TelemetryFields.LocationGroundSpeed] = 1000,
            [TelemetryFields.LocationYaw] = 270.0,
            [TelemetryFields.LocationPitch] = 0.0
        };

        var uav = new UAV
        {
            Id = "UAV-001",
            TelemetryData = telemetry,
            UAVType = UAVTypes.A1,
            WingId = 1,
            CurrentMissionId = "M-001"
        };

        var mission = new Mission(dest, 1);

        var service = new FlightPathService(uav, mission, logger);

        var tcs = new TaskCompletionSource();

        service.MissionCompleted += () => tcs.SetResult();

        service.StartFlightPath();

        await tcs.Task;

        return Ok("Simulation run complete. Check Output window for logs.");
    }
}