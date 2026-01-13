    using Microsoft.AspNetCore.Mvc;
using Core.Common.Enums;
using Core.Services.ICDsDirectory;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Models;
using Simulation.Models.Channels;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Models.UAVs.ArmedUav;
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
            Location startLocation = new Location(32.8000, 34.9900, 500.0);
            Location destination = new Location(31.8300, 34.9700, 1000.0);
            SimulateDto request = new SimulateDto(tailId: 2, destination, missionId: "armed-mission-converge");
            Searcher uav = new Searcher(tailId: 2, startLocation: startLocation);
            uav.TelemetryData[TelemetryFields.YawDeg] = 180.0;
            bool success = await _uavManager.StartMission(uav, destination, "armed-mission-converge");
            return success
                ? Ok("UAV 2 started successfully (same as run-multi)")
                : BadRequest("Mission failed to start");
        }

        [HttpGet("run-multi")]
        public async Task<IActionResult> RunMultipleUAVs()
        {
            Location armedStart = new Location(32.8000, 34.9900, 500.0);

            Location surveillanceStart = new Location(31.2500, 34.8000, 500.0);

            Location armedDestination = new Location(31.8300, 34.9700, 1000.0);

            Location surveillanceDestination = new Location(31.8305, 34.9705, 1000.0);

            HeronTp armedUav = new HeronTp(tailId: 2, startLocation: armedStart);
            armedUav.TelemetryData[TelemetryFields.YawDeg] = 180.0;

            Searcher surveillanceUav = new Searcher(tailId: 3, startLocation: surveillanceStart);
            surveillanceUav.TelemetryData[TelemetryFields.YawDeg] = 0.0;

            Task<bool> armedMissionTask = _uavManager.StartMission(armedUav, armedDestination, "armed-mission-converge");
            Task<bool> surveillanceMissionTask = _uavManager.StartMission(surveillanceUav, surveillanceDestination, "surveillance-mission-converge");

            bool[] results = await Task.WhenAll(armedMissionTask, surveillanceMissionTask);

            bool bothSuccessful = results[0] && results[1];
            return bothSuccessful
                ? Ok("Both UAVs started successfully and are converging.")
                : BadRequest("One or more missions failed to start");
        }
    }
}
