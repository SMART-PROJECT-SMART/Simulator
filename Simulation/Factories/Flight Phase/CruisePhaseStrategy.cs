using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Models.UAVs;

namespace Simulation.Factories.Flight_Phase
{
    public class CruisePhaseStrategy : IFlightPhaseStrategy
    {
        public FlightPhase Phase => FlightPhase.Cruise;

        public double ComputePitchDegrees(
            Location current,
            Location destination,
            double remainingKm)
        {
            return 0.0;
        }

        public double ComputeSpeedKmph(
            double currentSpeedKmph,
            double remainingKm,
            UAV uav,
            double deltaSec)
        {
            return uav.MaxCruiseSpeedKmph > 0
                ? uav.MaxCruiseSpeedKmph
                : currentSpeedKmph;
        }
    }
}