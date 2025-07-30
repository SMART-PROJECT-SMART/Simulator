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
            double deltaSec
        )
        {
            var speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            var pitchDeg = telemetry.GetValueOrDefault(TelemetryFields.PitchDeg, 0.0);

            double distToDestination = FlightPathMathHelper.CalculateDistance(current, destination);
            double speedMps = speedKmph.ToKmhFromMps();
            double travelM = speedMps * deltaSec;

            double horizM = travelM * Math.Cos(UnitConversionHelper.ToRadians(pitchDeg));
            if (horizM >= distToDestination)
                horizM = distToDestination;

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHoriz = FlightPathMathHelper.CalculateDestinationLocation(
                current,
                bearing,
                horizM
            );

            double newAlt = FlightPhysicsCalculator.CalculateAltitudeChange(
                travelM,
                pitchDeg,
                deltaSec,
                telemetry,
                current.Altitude
            );
            newAlt = Math.Min(destination.Altitude, newAlt);
            newAlt = Math.Max(0.0, newAlt);

            nextHoriz.Longitude = nextHoriz.Longitude.NormalizeAngle();

            return new Location(nextHoriz.Latitude, nextHoriz.Longitude, newAlt);
        }
    }
}
