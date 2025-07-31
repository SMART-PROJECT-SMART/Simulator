using Microsoft.AspNetCore.Mvc;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Services;

namespace Simulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly UAVManager _uavManager;

        public SimulationController(UAVManager uavManager)
        {
            _uavManager = uavManager;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
        {
            _uavManager.AddUAV(dto.UAV);

            var success = await _uavManager.StartMission(dto.UAV, dto.Destination);

            return success ? Ok() : BadRequest("Mission failed");
        }

        [HttpGet("run")]
        public async Task<IActionResult> Run()
        {
            var startLocation = new Location(40.6413, -73.7781, 10.0);
            var uav = new Searcher(tailId: 1, startLocation: startLocation);
            uav.TelemetryData[TelemetryFields.YawDeg] = 270.0;
            var destination = new Location(40.6460, -73.77850, 100.0);
            var request = new SimulateDto(uav, destination);

            return await CalculateFlightPath(request);
        }
    }
}
