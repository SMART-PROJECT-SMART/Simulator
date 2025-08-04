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

        public bool SwitchUAVs(int tailId1, int tailId2);
        public Task<bool> StartMission(UAV uav, Location destination, string missionId);
        public Task<bool> PauseMission(int tailId);
        public Task<bool> ResumeMission(int tailId);
        public Task<bool> AbortMission(int tailId);
        public Task<bool> AbortAllMissions();
        public int ActiveUAVCount { get; }
        public IEnumerable<int> GetActiveTailIds { get; }
        public Task<int> GetActiveJobCount();
    }
}
