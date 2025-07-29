using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Motion_Calculator
{

    public class MotionCalculator : IMotionCalculator
    {
        public Location CalculateNext(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec)
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double pitchDeg = telemetry.GetValueOrDefault(TelemetryFields.PitchDeg, 0.0);

            double dist = FlightPathMathHelper.CalculateDistance(current, destination);
            double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            double travelM = speedMps * deltaSec;

            if (dist < SimulationConstants.FlightPath.CLOSE_DISTANCE_M)
                return new Location(destination.Latitude, destination.Longitude, destination.Altitude);

            double horizM = travelM * Math.Cos(UnitConversionHelper.ToRadians(pitchDeg));
            if (horizM >= dist) horizM = dist;

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHoriz = FlightPathMathHelper.CalculateDestinationLocation(current, bearing, horizM);

            double newAlt;
            double altChange = travelM * Math.Sin(UnitConversionHelper.ToRadians(pitchDeg));
            newAlt = current.Altitude + altChange;

            newAlt = Math.Clamp(newAlt, 0, destination.Altitude);
            telemetry[TelemetryFields.Latitude] = nextHoriz.Latitude;
            telemetry[TelemetryFields.longitude] = nextHoriz.Longitude;
            telemetry[TelemetryFields.Altitude] = newAlt;

            return new Location(nextHoriz.Latitude, nextHoriz.Longitude, newAlt);
        }
    }
}
