using Microsoft.AspNetCore.Mvc;
using Core.Common.Enums;
using Core.Services.ICDsDirectory;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.Channels;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Services.UAVManager;

namespace Simulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IUAVManager _uavManager;
        private readonly IICDDirectory _ICDDirectory;

        public SimulationController(IUAVManager uavManager, IICDDirectory _icdDirectory)
        {
            _uavManager = uavManager;
            _ICDDirectory = _icdDirectory;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
        {
            Location startLocation = new Location(40.6413, -73.7781, 100.0);
            Searcher uav = new Searcher(tailId: dto.TailId, startLocation: startLocation);
            uav.TelemetryData[TelemetryFields.YawDeg] = 270.0;

            bool success = await _uavManager.StartMission(uav, dto.Destination, dto.MissionId);
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
            Location destination = new Location(40.6460, -73.77850, 10.0);
            SimulateDto request = new SimulateDto(tailId: 1, destination, missionId: "test-mission-1");

            return await CalculateFlightPath(request);
        }

        [HttpGet("run-multi")]
        public async Task<IActionResult> RunMultipleUAVs()
        {
            Location uav1Destination = new Location(40.6450, -73.7750, 120.0);
            Location uav2Destination = new Location(40.6480, -73.7720, 150.0);

            SimulateDto request1 = new SimulateDto(tailId: 1, uav1Destination, missionId: "test-mission-1");
            SimulateDto request2 = new SimulateDto(tailId: 2, uav2Destination, missionId: "test-mission-2");

            Task<IActionResult> task1 = CalculateFlightPath(request1);
            Task<IActionResult> task2 = CalculateFlightPath(request2);

            await Task.WhenAll(task1, task2);

            return Ok("Both UAVs started successfully");
        }
    }
}
