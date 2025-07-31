using Simulation.Models;

namespace Simulation.Dto.FlightPath
{
    public struct SwitchDestinationDto(int tailId, Location newDestination,string missionId)
    {
        public int  TailId { get; set; } = tailId;
        public Location NewDestination { get; set;} = newDestination;

        public string MissionId { get; set; } = missionId;
    }
}
