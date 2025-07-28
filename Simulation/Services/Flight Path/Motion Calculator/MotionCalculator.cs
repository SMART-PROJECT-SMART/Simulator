using Simulation.Models.Mission;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{
    public class MotionCalculator : IMotionCalculator
    {
        public Location CalculateNext(Location current, Location destination, double groundSpeedMps, double pitchDegrees,
            double deltaSeconds)
        {
            var remaining = FlightPathMathHelper.CalculateDistance(current, destination);
            var travel = groundSpeedMps * deltaSeconds;

            if (travel >= remaining)
                return destination;

            var bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var horizontal = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, travel);

            var altitudeDelta = Math.Sin(UnitConversionHelper.ToRadians(pitchDegrees)) * travel;

            return new Location(
                horizontal.Latitude,
                horizontal.Longitude,
                horizontal.Altitude + altitudeDelta);
        }
    }
}
