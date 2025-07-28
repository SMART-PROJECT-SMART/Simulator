using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{
    public class MotionCalculator : IMotionCalculator
    {
        public Location CalculateNext(
            Location current,
            Location destination,
            double speedKmph,
            double deltaHours)
        {
            double remainingKm = FlightPathMathHelper.CalculateDistance(current, destination) / 1000.0;
            double travelKm = speedKmph * deltaHours;
            if (travelKm >= remainingKm) return destination;

            double travelM = travelKm * 1000.0;
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            Location horizontal = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, travelM);


            double altDiff = destination.Altitude - current.Altitude;
            double climbRad = Math.Atan2(altDiff, remainingKm * 1000.0);
            double altDelta = Math.Sin(climbRad) * travelM;
            return new Location(
                horizontal.Latitude,
                horizontal.Longitude,
                current.Altitude + altDelta);
        }
    }
}