using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path;

namespace Simulation.Services
{
    public class UAVManager
    {
        private Dictionary<UAV, bool> _uavsStatus;
        private readonly FlightPathService _flightPathService;

        public UAVManager(FlightPathService flightPathService)
        {
            _uavsStatus = new Dictionary<UAV, bool>();
            _flightPathService = flightPathService;
        }

        public void AddUAV(UAV uav)
        {
            _uavsStatus[uav] = false;
        }

        public async Task<bool> StartMission(UAV uav, Location destination)
        {
            _flightPathService.Initialize(uav, destination);
            _flightPathService.StartFlightPath();

            var tcs = new TaskCompletionSource<bool>();
            _flightPathService.MissionCompleted += () =>
            {
                tcs.SetResult(true);
                uav.Land();
                _uavsStatus[uav] = true;
            };

            await tcs.Task;
            _flightPathService.Dispose();
            return true;
        }
    }
}
