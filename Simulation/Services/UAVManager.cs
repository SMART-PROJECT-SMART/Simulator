
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
        private Dictionary<int, UAV> _uavsStatus;
        private Dictionary<int, FlightPathService> _activeFlights;
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
            _uavsStatus = new Dictionary<int, UAV>();
            _activeFlights = new Dictionary<int, FlightPathService>();
            _motionCalculator = motionCalculator;
            _speedController = speedController;
            _orientationCalculator = orientationCalculator;
            _logger = logger;
        }

        public void AddUAV(UAV uav)
        {
            _uavsStatus[uav.TailId] = uav;
        }

        public void RemoveUAV(int tailId)
        {
            if (_activeFlights.TryGetValue(tailId, out var flightService))
            {
                flightService.Dispose();
                _activeFlights.Remove(tailId);
            }
            _uavsStatus.Remove(tailId);
        }

        public async Task<bool> StartMission(UAV uav, Location destination)
        {
            var flightService = new FlightPathService(
                _motionCalculator,
                _speedController,
                _orientationCalculator,
                _logger
            );

            _activeFlights[uav.TailId] = flightService;

            flightService.Initialize(uav, destination);
            flightService.StartFlightPath();

            var tcs = new TaskCompletionSource<bool>();
            flightService.MissionCompleted += () =>
            {
                tcs.SetResult(true);
                uav.Land();
                _activeFlights.Remove(uav.TailId);
            };

            await tcs.Task;
            flightService.Dispose();
            return true;
        }

        public bool SwitchDestination(int tailId, Location newDestination)
        {
            if (_activeFlights.TryGetValue(tailId, out var flightService))
            {
                flightService.SwitchDestination(newDestination);
                return true;
            }
            return false;
        }
    }
}
