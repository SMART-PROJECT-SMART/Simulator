using Core.Models;
using Core.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator.Helpers
{
    public class PitchCalculator
    {
        public double CalculatePhysicsBasedPitch(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination
        )
        {
            double altitudeDifference = destination.Altitude - current.Altitude;

            if (IsAltitudeWithinTolerance(altitudeDifference))
                return 0.0;

            double speedMps = GetSpeedInMps(telemetry);
            if (speedMps < SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double remainingDistance = FlightPathMathHelper.CalculateDistance(current, destination);
            if (IsAtDestination(remainingDistance, altitudeDifference))
                return 0.0;

            return CalculatePitchForAltitudeChange(telemetry, altitudeDifference, remainingDistance);
        }

        private bool IsAltitudeWithinTolerance(double altitudeDifference)
        {
            return Math.Abs(altitudeDifference) < SimulationConstants.FlightPath.ALTITUDE_TOLERANCE;
        }

        private double GetSpeedInMps(Dictionary<TelemetryFields, double> telemetry)
        {
            return telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0).ToKmhFromMps();
        }

        private bool IsAtDestination(double remainingDistance, double altitudeDifference)
        {
            return remainingDistance <= 0.0 && IsAltitudeWithinTolerance(altitudeDifference);
        }

        private double CalculatePitchForAltitudeChange(
            Dictionary<TelemetryFields, double> telemetry,
            double altitudeDifference,
            double remainingDistance
        )
        {
            if (altitudeDifference > 0)
                return CalculateClimbPitch(telemetry, altitudeDifference, remainingDistance);
            else if (altitudeDifference < 0)
                return CalculateDescentPitch(telemetry, altitudeDifference, remainingDistance);
            else
                return 0.0;
        }

        private double CalculateClimbPitch(
            Dictionary<TelemetryFields, double> telemetry,
            double altitudeDifference,
            double remainingDistance
        )
        {
            double horizontalSpeedMps = GetSpeedInMps(telemetry);

            if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
            {
                double requiredPitch = CalculateRequiredPitch(altitudeDifference, remainingDistance, horizontalSpeedMps);
                return Math.Clamp(requiredPitch, 0.0, SimulationConstants.FlightPath.MAX_CLIMB_DEG);
            }

            return SimulationConstants.FlightPath.MAX_CLIMB_DEG;
        }

        private double CalculateDescentPitch(
            Dictionary<TelemetryFields, double> telemetry,
            double altitudeDifference,
            double remainingDistance
        )
        {
            double horizontalSpeedMps = GetSpeedInMps(telemetry);

            if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
            {
                double requiredPitch = CalculateRequiredPitch(altitudeDifference, remainingDistance, horizontalSpeedMps);
                return Math.Clamp(requiredPitch, -SimulationConstants.FlightPath.MAX_DESCENT_DEG, 0.0);
            }

            return -SimulationConstants.FlightPath.MAX_DESCENT_DEG;
        }

        private double CalculateRequiredPitch(double altitudeDifference, double remainingDistance, double horizontalSpeedMps)
        {
            double effectiveDistance = Math.Max(remainingDistance, SimulationConstants.FlightPath.MIN_DISTANCE_M);
            double timeToReachTarget = Math.Max(effectiveDistance / horizontalSpeedMps, 2.0);
            double requiredVerticalSpeedMps = altitudeDifference / timeToReachTarget;

            double requiredPitchRad = Math.Atan2(requiredVerticalSpeedMps, horizontalSpeedMps);
            return requiredPitchRad.ToDegrees();
        }
    }
}
