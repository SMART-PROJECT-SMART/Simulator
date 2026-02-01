using Core.Models;
﻿using Microsoft.AspNetCore.Mvc;
using Core.Common.Enums;
using Core.Services.ICDsDirectory;
using Simulation.Common.Enums;
using Simulation.Dto.FlightPath;
using Simulation.Dto.DeviceManager;
using Simulation.Models;
using Simulation.Models.Channels;
using Simulation.Models.UAVs;
using Simulation.Models.UAVs.SurveillanceUAV;
using Simulation.Models.UAVs.ArmedUav;
using Simulation.Services.UAVManager;
using Simulation.Services.UAVStorage;
using Simulation.Services.UAVFactory;

namespace Simulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IUAVManager _uavManager;
        private readonly IICDDirectory _ICDDirectory;
        private readonly IUAVStorageService _uavStorageService;
        private readonly IUAVFactory _uavFactory;

        public SimulationController(IUAVManager uavManager, IICDDirectory _icdDirectory, IUAVStorageService uavStorageService, IUAVFactory uavFactory)
        {
            _uavManager = uavManager;
            _ICDDirectory = _icdDirectory;
            _uavStorageService = uavStorageService;
            _uavFactory = uavFactory;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> CalculateFlightPath([FromBody] SimulateDto dto)
        {
            DeviceManagerUAVDto uavDto = _uavStorageService.GetUAV(dto.TailId);

            if (uavDto == null)
            {
                return NotFound($"UAV with TailId {dto.TailId} does not exist");
            }

            UAV uav = _uavFactory.CreateUAV(uavDto, uavDto.BaseLocation);

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
            int tailId = 2;
            DeviceManagerUAVDto uavDto = _uavStorageService.GetUAV(tailId);

            if (uavDto == null)
            {
                return NotFound($"UAV with TailId {tailId} does not exist");
            }

            Location destination = new Location(31.8300, 34.9700, 1000.0);
            UAV uav = _uavFactory.CreateUAV(uavDto, uavDto.BaseLocation);
            bool success = await _uavManager.StartMission(uav, destination, "armed-mission-converge");
            return success
                ? Ok("UAV 2 started successfully")
                : BadRequest("Mission failed to start");
        }

        [HttpGet("run-multi")]
        public async Task<IActionResult> RunMultipleUAVs()
        {
            int armedTailId = 2;
            int surveillanceTailId = 3;

            DeviceManagerUAVDto armedUavDto = _uavStorageService.GetUAV(armedTailId);
            DeviceManagerUAVDto surveillanceUavDto = _uavStorageService.GetUAV(surveillanceTailId);

            if (armedUavDto == null)
            {
                return NotFound($"UAV with TailId {armedTailId} does not exist");
            }

            if (surveillanceUavDto == null)
            {
                return NotFound($"UAV with TailId {surveillanceTailId} does not exist");
            }

            Location armedDestination = new Location(31.8300, 34.9700, 1000.0);
            Location surveillanceDestination = new Location(31.8305, 34.9705, 1000.0);

            UAV armedUav = _uavFactory.CreateUAV(armedUavDto, armedUavDto.BaseLocation);
            UAV surveillanceUav = _uavFactory.CreateUAV(surveillanceUavDto, surveillanceUavDto.BaseLocation);

            Task<bool> armedMissionTask = _uavManager.StartMission(armedUav, armedDestination, "armed-mission-converge");
            Task<bool> surveillanceMissionTask = _uavManager.StartMission(surveillanceUav, surveillanceDestination, "surveillance-mission-converge");

            bool[] results = await Task.WhenAll(armedMissionTask, surveillanceMissionTask);

            bool bothSuccessful = results[0] && results[1];
            return bothSuccessful
                ? Ok("Both UAVs started successfully and are converging.")
                : BadRequest("One or more missions failed to start");
        }

        [HttpGet("run-20")]
        public async Task<IActionResult> RunTwentyUAVs()
        {
            Location destination = new Location(31.8300, 34.9700, 1000.0);
            List<Task<bool>> tasks = new List<Task<bool>>();

            for (int i = 4; i <= 24; i++)
            {
                DeviceManagerUAVDto uavDto = _uavStorageService.GetUAV(i);

                if (uavDto == null)
                {
                    return NotFound($"UAV with TailId {i} does not exist");
                }

                UAV uav = _uavFactory.CreateUAV(uavDto, uavDto.BaseLocation);
                tasks.Add(_uavManager.StartMission(uav, destination, $"multi-mission-{i}"));
            }

            bool[] results = await Task.WhenAll(tasks);
            bool allSuccessful = results.All(x => x);
            return allSuccessful
                ? Ok("All 20 UAVs started successfully and are converging.")
                : BadRequest("One or more missions failed to start");
        }
    }
}
