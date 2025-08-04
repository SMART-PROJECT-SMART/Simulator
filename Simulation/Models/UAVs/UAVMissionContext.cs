using Simulation.Services.Flight_Path;

namespace Simulation.Models.UAVs
{
    public class UAVMissionContext
    {
        public UAV UAV { get; set; }
        public FlightPathService Service { get; set; }

        public UAVMissionContext(UAV uav, FlightPathService service)
        {
            UAV = uav;
            Service = service;
        }
    }
}
