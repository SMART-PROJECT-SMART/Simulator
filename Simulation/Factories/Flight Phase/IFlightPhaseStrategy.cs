using Simulation.Common.Enums;
using Simulation.Models.Mission;

namespace Simulation.Factories.Flight_Phase
{
    public interface IFlightPhaseStrategy
    {
        FlightPhase Phase { get; }
        double ComputePitchDegrees(
            Location current,
            Location destination,
            double remainingKm);
        double ComputeSpeedKmph(
            double currentSpeedKmph,
            double remainingKm,
            UAV uav,
            double deltaSec);
    }
}