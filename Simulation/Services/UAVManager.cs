
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using System.Collections.Concurrent;

namespace Simulation.Services
{
    public class UAVManager
    {
        private readonly ConcurrentDictionary<int, UAVMissionContext> _uavs;
        private readonly IMotionCalculator _motionCalculator;
        private readonly ISpeedController _speedController;
        private readonly IOrientationCalculator _orientationCalculator;
        private readonly ILogger<FlightPathService> _logger;

        public UAVManager(
            IMotionCalculator motionCalculator,
            ISpeedController speedController,
            IOrientationCalculator orientationCalculator,
            ILogger<FlightPathService> logger)
        {
            _uavs = new ConcurrentDictionary<int, UAVMissionContext>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
        }

        public void AddUAV(UAV uav)
        {
            var flightService = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );
            
            _uavs.TryAdd(uav.TailId,new UAVMissionContext(uav,flightService));
        }

        public void RemoveUAV(int tailId)
        {
            if (_uavs.TryRemove(tailId, out var uavData))
            {
                uavData.Service.Dispose();
            }
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

            var tcs = new TaskCompletionSource<bool>();
            context.Service.MissionCompleted += () =>
            {
                tcs.SetResult(true);
                uav.CurrentMissionId = string.Empty;
            };

            await tcs.Task;
            context.Service.Dispose();
            _uavs.TryRemove(uav.TailId, out _);
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

        public int ActiveUAVCount => _uavs.Count;
        
        public IEnumerable<int> GetActiveTailIds => _uavs.Keys;
    }
}
