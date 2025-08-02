using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Models;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;

namespace Simulation.Services.Flight_Path.Orientation_Calculator
{
    public class OrientationCalculator : IOrientationCalculator
    {
        private double _lastYaw = double.NaN;
        private double _lastRoll = 0.0;

        public AxisDegrees ComputeOrientation(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double pitch = CalculatePhysicsBasedPitch(telemetry, current, destination);
            double currentYaw = telemetry.GetValueOrDefault(TelemetryFields.YawDeg, 0.0);
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double newYaw = CalculateNewYaw(currentYaw, bearing, deltaSec);
            double roll = CalculatePhysicsBasedRoll(
                telemetry,
                current,
                destination,
                deltaSec,
                newYaw
            );
            _lastYaw = newYaw;
            return new AxisDegrees(newYaw, pitch, roll);
        }

        private double CalculatePhysicsBasedPitch(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination
        )
        {
            double altitudeDifference = destination.Altitude - current.Altitude;

            if (Math.Abs(altitudeDifference) < SimulationConstants.FlightPath.ALTITUDE_TOLERANCE)
                return 0.0;

            double speedMps = telemetry
                .GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0)
                .ToKmhFromMps();
            if (speedMps < SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double remainingDistance = FlightPathMathHelper.CalculateDistance(current, destination);
            if (
                remainingDistance <= 0.0
                && Math.Abs(altitudeDifference) < SimulationConstants.FlightPath.ALTITUDE_TOLERANCE
            )
                return 0.0;

            double pitch;

            if (altitudeDifference > 0)
            {
                double currentSpeedKmph = telemetry.GetValueOrDefault(
                    TelemetryFields.CurrentSpeedKmph,
                    0.0
                );
                double horizontalSpeedMps =
                    currentSpeedKmph.ToKmhFromMps();

                if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
                {
                    double effectiveDistance = Math.Max(
                        remainingDistance,
                        SimulationConstants.FlightPath.MIN_DISTANCE_M
                    );
                    double timeToReachTarget = Math.Max(
                        effectiveDistance / horizontalSpeedMps,
                        2.0
                    );
                    double requiredVerticalSpeedMps = altitudeDifference / timeToReachTarget;

                    double requiredPitchRad = Math.Atan2(
                        requiredVerticalSpeedMps,
                        horizontalSpeedMps
                    );
                    pitch = requiredPitchRad.ToDegrees();

                    pitch = Math.Clamp(pitch, 0.0, SimulationConstants.FlightPath.MAX_CLIMB_DEG);
                }
                else
                {
                    pitch = SimulationConstants.FlightPath.MAX_CLIMB_DEG;
                }
            }
            else if (altitudeDifference < 0)
            {
                double currentSpeedKmph = telemetry.GetValueOrDefault(
                    TelemetryFields.CurrentSpeedKmph,
                    0.0
                );
                double horizontalSpeedMps = currentSpeedKmph;

                if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS)
                {
                    double effectiveDistance = Math.Max(
                        remainingDistance,
                        SimulationConstants.FlightPath.MIN_DISTANCE_M
                    );
                    double timeToReachTarget = Math.Max(
                        effectiveDistance / horizontalSpeedMps,
                        2.0
                    );
                    double requiredVerticalSpeedMps = altitudeDifference / timeToReachTarget;

                    double requiredPitchRad = Math.Atan2(
                        requiredVerticalSpeedMps,
                        horizontalSpeedMps
                    );
                    pitch = UnitConversionHelper.ToDegrees(requiredPitchRad);

                    pitch = Math.Clamp(pitch, -SimulationConstants.FlightPath.MAX_DESCENT_DEG, 0.0);
                }
                else
                {
                    pitch = -SimulationConstants.FlightPath.MAX_DESCENT_DEG;
                }
            }
            else
            {
                pitch = 0.0;
            }

            return pitch;
        }

        private double CalculatePhysicsBasedRoll(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec,
            double newYaw
        )
        {
            double speedMps = telemetry
                .GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0)
                .ToKmhFromMps();
            if (speedMps <= SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double targetRoll = 0.0;
            if (!double.IsNaN(_lastYaw))
            {
                double dYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, newYaw);
                double yawRate = dYaw.ToRadians() / deltaSec;
                if (Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE)
                {
                    double latAcc = speedMps * yawRate;
                    targetRoll = Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2).ToDegrees();
                    targetRoll = Math.Clamp(
                        targetRoll,
                        -SimulationConstants.FlightPath.MAX_ROLL_DEG,
                        SimulationConstants.FlightPath.MAX_ROLL_DEG
                    );
                }
                targetRoll = CalculateCurveRoll(current, destination, newYaw, targetRoll);
            }

            double maxRollRate = SimulationConstants.FlightPath.MAX_ROLL_RATE_DEG_PER_SEC;
            double maxRollDelta = maxRollRate * deltaSec;
            double rollDiff = targetRoll - _lastRoll;

            if (Math.Abs(rollDiff) <= maxRollDelta)
            {
                _lastRoll = targetRoll;
            }
            else
            {
                _lastRoll += Math.Sign(rollDiff) * maxRollDelta;
            }

            return _lastRoll;
        }

        private double CalculateNewYaw(double currentYaw, double targetYaw, double deltaSec)
        {
            double diff = FlightPathMathHelper.CalculateAngleDifference(currentYaw, targetYaw);
            double turnRate = SimulationConstants.FlightPath.MAX_TURN_RATE_DEG_PER_SEC;
            double maxDelta = turnRate * deltaSec;
            double newYaw;
            if (Math.Abs(diff) <= maxDelta)
                newYaw = targetYaw;
            else
                newYaw = currentYaw + Math.Sign(diff) * maxDelta;
            
            return newYaw.NormalizeAngle();
        }

        private double CalculateCurveRoll(
            Location current,
            Location destination,
            double newYaw,
            double currentRoll
        )
        {
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double diff = FlightPathMathHelper.CalculateAngleDifference(newYaw, bearing);
            if (Math.Abs(diff) > SimulationConstants.FlightPath.CURVE_ROLL_THRESHOLD_DEG)
            {
                double curveRoll = Math.Min(
                    Math.Abs(diff) * SimulationConstants.FlightPath.CURVE_ROLL_MULTIPLIER,
                    SimulationConstants.FlightPath.MAX_CURVE_ROLL_DEG
                );
                return currentRoll + Math.Sign(diff) * curveRoll;
            }
            return currentRoll;
        }
    }
}
