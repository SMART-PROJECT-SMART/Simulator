using Simulation.Common.constants;
using Simulation.Models;
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
            double initialDistanceM = FlightPathMathHelper.CalculateDistance(current, destination);

            if (initialDistanceM < SimulationConstants.FlightPath.CLOSE_DISTANCE_M)
            {
                return new Location(destination.Latitude, destination.Longitude, targetAltitude);
            }

            double speedMps = speedKmph / 3.6;
            double totalTravelM = speedMps * (deltaHours * 3600.0);

            double horizontalTravelM;
            if (Math.Abs(pitchDegrees) > 0.1)
            {
                double pitchRad = UnitConversionHelper.ToRadians(pitchDegrees);
                horizontalTravelM = totalTravelM * Math.Cos(pitchRad);
            }
            else
            {
                horizontalTravelM = totalTravelM;
            }

            if (horizontalTravelM >= initialDistanceM)
            {
                return new Location(destination.Latitude, destination.Longitude, targetAltitude);
            }

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHorizontalLocation = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, horizontalTravelM);

            double newAltitude = CalculateNewAltitude(current, totalTravelM, pitchDegrees, targetAltitude, initialDistanceM);

            return new Location(nextHorizontalLocation.Latitude, nextHorizontalLocation.Longitude, newAltitude);
        }

        private static double CalculateNewAltitude(Location current, double totalTravelM, double pitchDegrees, double targetAltitude, double remainingDistanceM)
        {
            if (Math.Abs(pitchDegrees) < 0.1)
            {
                double altDiff = targetAltitude - current.Altitude;
                if (Math.Abs(altDiff) < SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
                {
                    return current.Altitude;
                }

                double maxAltChange = totalTravelM * 0.1;
                double altChange = Math.Sign(altDiff) * Math.Min(Math.Abs(altDiff), maxAltChange);
                return current.Altitude + altChange;
            }

            double pitchRad = UnitConversionHelper.ToRadians(pitchDegrees);
            double altitudeChangeM = totalTravelM * Math.Sin(pitchRad);
            double newAltitude = current.Altitude + altitudeChangeM;

            if (pitchDegrees > 0)
            {
                newAltitude = Math.Min(newAltitude, targetAltitude);
            }
            else if (pitchDegrees < 0)
            {
                newAltitude = Math.Max(newAltitude, targetAltitude);
            }

            return Math.Max(0, newAltitude);
        }
    }
}