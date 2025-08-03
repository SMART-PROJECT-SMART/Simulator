using Simulation.Models;
using Simulation.Models.UAVs;

namespace Simulation.Services.UAVManager
{
    public interface IUAVManager
    {
        public void AddUAV(UAV uav);
        public void RemoveUAV(int tailId);
        public UAVMissionContext? GetUAVContext(int tailId);
        public bool SwitchDestination(int tailId, Location newDestination);
        public Task<bool> StartMission(UAV uav, Location destination, string missionId);
        public int ActiveUAVCount { get; }
        public IEnumerable<int> GetActiveTailIds { get; }
    }
}
