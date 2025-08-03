using System.Collections.Concurrent;
using Quartz;
using Simulation.Common.constants;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.Quartz;

namespace Simulation.Services.UAVManager
{
    public class UAVManager : IUAVManager
    {
        private readonly ConcurrentDictionary<int, UAVMissionContext?> _uavs;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly ILogger<FlightPathService> _logger;
        private readonly IQuartzManager _quartzManager;

        public UAVManager(
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger,
            IQuartzManager quartzManager
        )
        {
            _uavs = new ConcurrentDictionary<int, UAVMissionContext?>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
            _quartzManager = quartzManager;
        }

        public void AddUAV(UAV uav)
        {
            var flightService = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );

            _uavs.TryAdd(uav.TailId, new UAVMissionContext(uav, flightService));
        }

        public void RemoveUAV(int tailId)
        {
            if (_uavs.TryRemove(tailId, out var uavData))
            {
                uavData.Service.Dispose();
            }
        }

        public UAVMissionContext? GetUAVContext(int tailId)
        {
            return _uavs.GetValueOrDefault(tailId);
        }

        public bool SwitchUAVs(int tailId1, int tailId2)
        {
            var tail1Destination = GetUAVContext(tailId1).Service.GetDestination();
            var tail2Destination = GetUAVContext(tailId2).Service.GetDestination();

            return SwitchDestination(tailId1, tail2Destination)
                && SwitchDestination(tailId2, tail1Destination);
        }

        public async Task<bool> StartMission(UAV uav, Location destination, string missionId)
        {
            uav.CurrentMissionId = missionId;
            if (!_uavs.ContainsKey(uav.TailId))
            {
                AddUAV(uav);
            }

            UAVMissionContext context = _uavs[uav.TailId];
            context.Service.Initialize(uav, destination);
            context.Service.StartFlightPath();

            // Use QuartzManager to schedule the job
            var jobScheduled = await _quartzManager.ScheduleUAVFlightPathJob(
                uav.TailId,
                (int)SimulationConstants.FlightPath.DELTA_SECONDS
            );

            if (!jobScheduled)
            {
                _logger.LogError("Failed to schedule flight path job for UAV {TailId}", uav.TailId);
                return false;
            }

            // Set up mission completion handler
            context.Service.MissionCompleted += async () =>
            {
                await _quartzManager.DeleteUAVFlightPathJob(uav.TailId);
                uav.CurrentMissionId = string.Empty;
                context.Service.Dispose();
                _uavs.TryRemove(uav.TailId, out _);
            };

            return true;
        }

        public bool SwitchDestination(int tailId, Location newDestination)
        {
            if (_uavs.TryGetValue(tailId, out var uavData))
            {
                uavData.Service.SwitchDestination(newDestination);
                return true;
            }
            return false;
        }

        public async Task<bool> PauseMission(int tailId)
        {
            if (_uavs.ContainsKey(tailId))
            {
                return await _quartzManager.PauseUAVFlightPathJob(tailId);
            }
            return false;
        }

        public async Task<bool> ResumeMission(int tailId)
        {
            if (_uavs.ContainsKey(tailId))
            {
                return await _quartzManager.ResumeUAVFlightPathJob(tailId);
            }
            return false;
        }

        public async Task<bool> AbortMission(int tailId)
        {
            if (_uavs.TryGetValue(tailId, out var uavData))
            {
                // Delete the scheduled job
                var jobDeleted = await _quartzManager.DeleteUAVFlightPathJob(tailId);

                // Clean up UAV context
                uavData.UAV.CurrentMissionId = string.Empty;
                uavData.Service.Dispose();
                _uavs.TryRemove(tailId, out _);

                return jobDeleted;
            }
            return false;
        }

        public async Task<bool> AbortAllMissions()
        {
            var allJobsDeleted = await _quartzManager.DeleteAllJobs();

            // Clean up all UAV contexts
            foreach (var kvp in _uavs)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.UAV.CurrentMissionId = string.Empty;
                    kvp.Value.Service.Dispose();
                }
            }

            _uavs.Clear();
            return allJobsDeleted;
        }

        public async Task<int> GetActiveJobCount()
        {
            return await _quartzManager.GetActiveJobCount();
        }

        public int ActiveUAVCount => _uavs.Count;

        public IEnumerable<int> GetActiveTailIds => _uavs.Keys;
    }
}
