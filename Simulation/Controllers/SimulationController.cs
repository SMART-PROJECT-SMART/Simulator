using Microsoft.AspNetCore.Mvc;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Services.UAVManager;

namespace Simulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IUAVManager _uavManager;

        public SimulationController(IUAVManager uavManager)
        {
            _uavManager = uavManager;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
        {
            _uavManager.AddUAV(dto.UAV);
            var success = await _uavManager.StartMission(dto.UAV, dto.Destination, dto.MissionId);
            return success
                ? Ok("Mission started successfully")
                : BadRequest("Mission failed to start");
        }

        [HttpPost("switch")]
        public async Task<IActionResult> SwitchUAV([FromBody] SwitchDestinationDto dto)
        {
            var success = _uavManager.SwitchDestination(dto.TailId, dto.NewDestination);
            return success ? Ok() : BadRequest("Switch failed");
        }

        [HttpPost("pause/{tailId}")]
        public async Task<IActionResult> PauseMission(int tailId)
        {
            var success = await _uavManager.PauseMission(tailId);
            return success
                ? Ok($"Mission for UAV {tailId} paused successfully")
                : BadRequest($"Failed to pause mission for UAV {tailId}");
        }

        [HttpPost("resume/{tailId}")]
        public async Task<IActionResult> ResumeMission(int tailId)
        {
            var success = await _uavManager.ResumeMission(tailId);
            return success
                ? Ok($"Mission for UAV {tailId} resumed successfully")
                : BadRequest($"Failed to resume mission for UAV {tailId}");
        }

        [HttpPost("abort/{tailId}")]
        public async Task<IActionResult> AbortMission(int tailId)
        {
            var success = await _uavManager.AbortMission(tailId);
            return success
                ? Ok($"Mission for UAV {tailId} aborted successfully")
                : BadRequest($"Failed to abort mission for UAV {tailId}");
        }

        [HttpPost("abort-all")]
        public async Task<IActionResult> AbortAllMissions()
        {
            var success = await _uavManager.AbortAllMissions();
            return success
                ? Ok("All missions aborted successfully")
                : BadRequest("Failed to abort all missions");
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var activeUAVs = _uavManager.ActiveUAVCount;
            var activeJobs = await _uavManager.GetActiveJobCount();
            var activeTailIds = _uavManager.GetActiveTailIds.ToList();

            return Ok(
                new
                {
                    ActiveUAVs = activeUAVs,
                    ActiveJobs = activeJobs,
                    ActiveTailIds = activeTailIds,
                }
            );
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

        [HttpGet("run-multi")]
        public async Task<IActionResult> RunMultipleUAVs()
        {
            var uav1StartLocation = new Location(40.6413, -73.7781, 10.0);
            var uav1 = new Searcher(tailId: 1, startLocation: uav1StartLocation);
            uav1.TelemetryData[TelemetryFields.YawDeg] = 270.0;
            var uav1Destination = new Location(40.6450, -73.7750, 120.0);

            var uav2StartLocation = new Location(40.6400, -73.7800, 15.0);
            var uav2 = new Searcher(tailId: 2, startLocation: uav2StartLocation);
            uav2.TelemetryData[TelemetryFields.YawDeg] = 90.0;
            var uav2Destination = new Location(40.6480, -73.7720, 150.0);

            var request1 = new SimulateDto(uav1, uav1Destination);
            var request2 = new SimulateDto(uav2, uav2Destination);

            var task1 = CalculateFlightPath(request1);
            var task2 = CalculateFlightPath(request2);

            await Task.WhenAll(task1, task2);

            return Ok("Both UAVs started successfully");
        }
    }
}
