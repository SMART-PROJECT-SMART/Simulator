using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Models.UAVs;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.helpers;

namespace Simulation.Factories.Flight_Phase
{
    public class DescentPhaseStrategy : IFlightPhaseStrategy
    {
        public FlightPhase Phase => FlightPhase.Descent;

        public double ComputePitchDegrees(
            Location current,
            Location destination,
            double remainingKm)
        {
            double altDiff = destination.Altitude - current.Altitude;
            double horizMeters = remainingKm * 1000.0;
            return UnitConversionHelper.ToDegrees(
                Math.Atan2(altDiff, horizMeters));
        }

        public double ComputeSpeedKmph(
            double currentSpeedKmph,
            double remainingKm,
            UAV uav,
            double deltaSec)
        {
            return new SpeedController().ComputeNextSpeed(
                currentSpeedKmph,
                remainingKm,
                uav.MaxAcceleration,
                uav.MaxDeceleration,
                deltaSec,
                uav.MaxCruiseSpeedKmph);
        }
    }
}
