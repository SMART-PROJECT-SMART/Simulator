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

            double dist = FlightPathMathHelper.CalculateDistance(current, destination);
            double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            double travelM = speedMps * deltaSec;

            double horizM = travelM * Math.Cos(UnitConversionHelper.ToRadians(pitchDeg));
            if (horizM >= dist)
                horizM = dist;
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            var nextHoriz = FlightPathMathHelper.CalculateDestinationLocation(
                current,
                bearing,
                horizM
            );

            double altChange = travelM * Math.Sin(UnitConversionHelper.ToRadians(pitchDeg));

            double lift = FlightPhysicsCalculator.CalculateLift(telemetry);
            double liftContribution =
                lift * deltaSec * SimulationConstants.Mathematical.FROM_M_TO_KM;
            altChange += liftContribution;

            double drag = FlightPhysicsCalculator.CalculateDrag(telemetry);
            double dragEffect =
                -drag * deltaSec * SimulationConstants.FlightPath.DRAG_EFFECT_ON_ALTITUDE;
            altChange += dragEffect;

            double newAlt = current.Altitude + altChange;
            newAlt = Math.Min(destination.Altitude, newAlt);
            newAlt = Math.Max(0.0, newAlt);

            nextHoriz.Longitude = FlightPathMathHelper.NormalizeAngle(nextHoriz.Longitude);

            return new Location(nextHoriz.Latitude, nextHoriz.Longitude, newAlt);
        }
    }
}
