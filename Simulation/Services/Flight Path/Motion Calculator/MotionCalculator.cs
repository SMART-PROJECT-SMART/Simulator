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
            double deltaSec,
            double wingSurface,
            double frontalSurface
        )
        {
            var speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            var pitchDeg = telemetry.GetValueOrDefault(TelemetryFields.PitchDeg, 0.0);

            double distToDestination = FlightPathMathHelper.CalculateDistance(current, destination);
            double speedMps = speedKmph.ToKmhFromMps();
            double travelM = speedMps * deltaSec;

            double horizM = FlightPathMathHelper.GetHorizontalPart(travelM, pitchDeg);
            if (horizM >= distToDestination)
                horizM = distToDestination;

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHoriz = FlightPathMathHelper.CalculateDestinationLocation(
                current,
                bearing,
                horizM
            );

            double rawAlt = FlightPhysicsCalculator.CalculateAltitudeChange(
                travelM,
                pitchDeg,
                deltaSec,
                telemetry,
                current.Altitude,
                wingSurface,
                frontalSurface
            );

            double altChange = rawAlt - current.Altitude;
            double altDiff = destination.Altitude - current.Altitude;
            double newAlt;
            if (Math.Abs(altChange) >= Math.Abs(altDiff))
                newAlt = destination.Altitude;
            else
                newAlt = current.Altitude + altChange;

            nextHoriz.Longitude = nextHoriz.Longitude.NormalizeAngle();

            return new Location(nextHoriz.Latitude, nextHoriz.Longitude, newAlt);
        }
    }
}
