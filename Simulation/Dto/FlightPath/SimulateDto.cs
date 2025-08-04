using Simulation.Models;
using Simulation.Models.UAVs;

namespace Simulation.Dto.FlightPath
{
    public class SimulateDto
    {
        public UAV UAV { get; set; }
        public Location Destination { get; set; }
        public string MissionId { get; set; } = string.Empty;

        public SimulateDto() { }

        public SimulateDto(UAV uav, Location destination)
        {
            UAV = uav;
            Destination = destination;
        }
    }
}
