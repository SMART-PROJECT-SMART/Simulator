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
            double horizontalTravelKm = speedKmph * deltaHours;
            double horizontalTravelM = horizontalTravelKm * 1000.0;

            double initialDistanceM = FlightPathMathHelper.CalculateDistance(current, destination);
            if (horizontalTravelM >= initialDistanceM)
            {
                return new Location(destination.Latitude, destination.Longitude, destination.Altitude);
            }

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHorizontalLocation = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, horizontalTravelM);

            double pitchRad = UnitConversionHelper.ToRadians(pitchDegrees);
            double altitudeChangeM = horizontalTravelM * Math.Tan(pitchRad);
            double newAltitude = current.Altitude + altitudeChangeM;

            if (altitudeChangeM > 0)
            {
                newAltitude = Math.Min(newAltitude, targetAltitude);
            }
            else if (altitudeChangeM < 0)
            {
                newAltitude = Math.Max(newAltitude, targetAltitude);
            }

            return new Location(nextHorizontalLocation.Latitude, nextHorizontalLocation.Longitude, newAltitude);
        }
    }
}