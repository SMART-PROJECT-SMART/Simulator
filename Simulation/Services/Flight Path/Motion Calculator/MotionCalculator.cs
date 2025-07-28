using Simulation.Models.Mission;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{
    public class MotionCalculator : IMotionCalculator
    {
        public Location CalculateNext(
            Location current,
            Location destination,
            double speedKmph,
            double deltaHours,
            double pitchDegrees,
            double targetAltitude)
        {
            double remainingKm = FlightPathMathHelper.CalculateDistance(current, destination) / 1000.0;
            double travelKm = speedKmph * deltaHours;
            if (travelKm >= remainingKm)
                return new Location(destination.Latitude, destination.Longitude, targetAltitude);

            double travelM = travelKm * 1000.0;
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var horizontal = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, travelM);

            double pitchRad = UnitConversionHelper.ToRadians(pitchDegrees);
            double altDelta = Math.Sin(pitchRad) * travelM;
            double newAltitude = current.Altitude + altDelta;

            return new Location(horizontal.Latitude, horizontal.Longitude, newAltitude);
        }
    }
}