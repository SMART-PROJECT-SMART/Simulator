
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;

namespace Simulation.Services
{
    public class UAVManager
    {
        private Dictionary<UAV, FlightPathService> _uavs;
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
            _uavs = new Dictionary<UAV, FlightPathService>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
        }

        public void AddUAV(UAV uav)
        {
            _uavs[uav] = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );
        }

        public void RemoveUAV(int tailId)
        {
            foreach (var pair in _uavs.Where(pair => pair.Key.TailId == tailId))
            {
                _uavs[pair.Key].Dispose();
                break;
            }
        }

        public async Task<bool> StartMission(UAV uav, Location destination,string missionId)
        {
            uav.CurrentMissionId = missionId;

            if (!_uavs.ContainsKey(uav))
            {
                AddUAV(uav);
            }

            var flightService = _uavs[uav];
            flightService.Initialize(uav, destination);
            flightService.StartFlightPath();

            var tcs = new TaskCompletionSource<bool>();
            flightService.MissionCompleted += () =>
            {
                tcs.SetResult(true);
                uav.Land();
                uav.CurrentMissionId = string.Empty;
            };

            await tcs.Task;
            flightService.Dispose();
            _uavs.Remove(uav);
            return true;
        }

        public bool SwitchDestination(int tailId, Location newDestination)
        {
            foreach (var pair in _uavs.Where(pair => pair.Key.TailId == tailId))
            {
                pair.Value.SwitchDestination(newDestination);
                return true;
            }
            return false;
        }
    }
}
