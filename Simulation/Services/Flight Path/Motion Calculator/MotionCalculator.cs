using Simulation.Common.constants;
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
            
            if (horizontalTravelM >= initialDistanceM || initialDistanceM < SimulationConstants.FlightPath.CLOSE_DISTANCE_M)
            {
                return destination;
            }

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHorizontalLocation = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, horizontalTravelM);
            double newAltitude = CalculateNewAltitude(current, horizontalTravelM, pitchDegrees, targetAltitude);

            return new Location(nextHorizontalLocation.Latitude, nextHorizontalLocation.Longitude, newAltitude);
        }

        private static double CalculateNewAltitude(Location current, double horizontalTravelM, double pitchDegrees, double targetAltitude)
        {
            if (Math.Abs(pitchDegrees) < 0.1)
            {
                return current.Altitude;
            }

            double pitchRad = UnitConversionHelper.ToRadians(pitchDegrees);
            double altitudeChangeM = horizontalTravelM * Math.Tan(pitchRad);
            double newAltitude = current.Altitude + altitudeChangeM;

            if (pitchDegrees > 0)
            {
                newAltitude = Math.Min(newAltitude, targetAltitude);
            }
            else if (pitchDegrees < 0)
            {
                newAltitude = Math.Max(newAltitude, targetAltitude);
            }

            return newAltitude;
        }
    }
}