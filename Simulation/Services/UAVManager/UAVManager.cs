using Quartz;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Models.Channels;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.Helpers;
using Simulation.Services.ICDDirectory;
using Simulation.Services.Quartz;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Simulation.Services.PortManager;
using Channel = Simulation.Models.Channels.Channel;

namespace Simulation.Services.UAVManager
{
    public class UAVManager : IUAVManager
    {
        private readonly ConcurrentDictionary<int, UAVMissionContext> _uavs;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly ILogger<FlightPathService> _logger;
        private readonly IQuartzFlightJobManager _quartzFlightJobManager;
        private readonly IPortManager _portManager;

        public UAVManager(
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger,
            IQuartzFlightJobManager quartzFlightJobManager,
            IPortManager portManager
        )
        {
            _uavs = new ConcurrentDictionary<int, UAVMissionContext>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
            _quartzFlightJobManager = quartzFlightJobManager;
            _portManager = portManager;
        }

        public void AddUAV(UAV uav)
        {
            var flightService = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );

            flightService.TelemetryUpdated += (telemetry) => SendTelemetryToChannels(uav.TailId, telemetry);

            _uavs.TryAdd(uav.TailId, new UAVMissionContext(uav, flightService));
            foreach (var channel in uav.Channels)
            {
                _portManager.AssignPort(channel, channel.PortNumber);
            }
        }

        public void RemoveUAV(int tailId)
        {
            if (_uavs.TryRemove(tailId, out var uavData))
            {
                uavData.Service.Dispose();
            }
        }

        public UAVMissionContext GetUAVContext(int tailId)
        {
            return _uavs[tailId];
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

            var jobScheduled = await _quartzFlightJobManager.ScheduleUAVFlightPathJob(
                uav.TailId,
                (int)SimulationConstants.FlightPath.DELTA_SECONDS
            );

            if (!jobScheduled)
            {
                _logger.LogError("Failed to schedule flight path job for UAV {TailId}", uav.TailId);
                return false;
            }

            context.Service.MissionCompleted += async () =>
            {
                await _quartzFlightJobManager.DeleteUAVFlightPathJob(uav.TailId);
                uav.CurrentMissionId = string.Empty;
                context.Service.Dispose();
                RemoveUAV(uav);
            };
            return true;
        }

        private void RemoveUAV(UAV uav)
        {
            _uavs.TryRemove(uav.TailId, out _);
            uav.CurrentMissionId = string.Empty;
            uav.Channels.ForEach(channel => _portManager.RemovePort(channel.PortNumber));
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
                return await _quartzFlightJobManager.PauseUAVFlightPathJob(tailId);
            }
            return false;
        }

        public async Task<bool> ResumeMission(int tailId)
        {
            if (_uavs.ContainsKey(tailId))
            {
                return await _quartzFlightJobManager.ResumeUAVFlightPathJob(tailId);
            }
            return false;
        }

        public async Task<bool> AbortMission(int tailId)
        {
            if (!_uavs.TryGetValue(tailId, out var uavData)) return false;
            var jobDeleted = await _quartzFlightJobManager.DeleteUAVFlightPathJob(tailId);

            uavData.UAV.CurrentMissionId = string.Empty;
            uavData.Service.Dispose();
            RemoveUAV(uavData.UAV);

            return jobDeleted;
        }

        public async Task<bool> AbortAllMissions()
        {
            var allJobsDeleted = await _quartzFlightJobManager.DeleteAllJobs();

            foreach (var kvp in _uavs)
            {
                kvp.Value.UAV.CurrentMissionId = string.Empty;
                kvp.Value.Service.Dispose();
            }

            _uavs.Clear();
            _portManager.ClearPorts();
            return allJobsDeleted;
        }

        public async Task<int> GetActiveJobCount()
        {
            return await _quartzFlightJobManager.GetActiveJobCount();
        }

        public int ActiveUAVCount => _uavs.Count;

        public IEnumerable<int> GetActiveTailIds => _uavs.Keys;

    private void SendTelemetryToChannels(int tailId, Dictionary<TelemetryFields, double> telemetry)
    {
        foreach (var channel in _uavs[tailId].UAV.Channels)
        {
            BitArray compressed = TelemetryCompressionHelper.CompressTelemetryDataByICD(telemetry, channel.ICD);
            channel.SendICDByteArray(compressed);
        }
    }
}
}
