using Simulation.Models.Mission;
using Simulation.Services.Flight_Path;

namespace Simulation.Factories
{
    public interface IFlightPhaseFactory
    {
        FlightPathService Create(UAV uav, Mission mission);
    }
}
