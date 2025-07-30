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

        public AxisDegrees ComputeOrientation(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double pitch = CalculatePhysicsBasedPitch(telemetry, current, destination, deltaSec);
            double currentYaw = telemetry.GetValueOrDefault(TelemetryFields.YawDeg, 0.0);

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double newYaw = CalculateNewYaw(currentYaw, bearing, deltaSec);
            double roll = CalculatePhysicsBasedRoll(
                telemetry,
                previous,
                current,
                destination,
                deltaSec,
                newYaw,
                pitch
            );

            _lastYaw = newYaw;
            return new AxisDegrees(newYaw, pitch, roll);
        }

        private double CalculatePhysicsBasedPitch(
            Dictionary<TelemetryFields, double> telemetry,
            Location current,
            Location destination,
            double deltaSec
        )
        {
            double currentAltitude = current.Altitude;
            double targetAltitude = destination.Altitude;
            double altitudeDifference = targetAltitude - currentAltitude;

            double horizontalSpeedKmph = telemetry.GetValueOrDefault(
                TelemetryFields.CurrentSpeedKmph,
                0.0
            );
            double horizontalSpeedMps =
                horizontalSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

            if (horizontalSpeedMps < SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double remainingDistance = FlightPathMathHelper.CalculateDistance(current, destination);
            double requiredVerticalVelocity = CalculateRequiredVerticalVelocity(
                altitudeDifference,
                remainingDistance,
                horizontalSpeedMps
            );

            double basicPitch = Math.Atan2(requiredVerticalVelocity, horizontalSpeedMps);
            basicPitch = UnitConversionHelper.ToDegrees(basicPitch);

            double physicsAdjustedPitch = ApplyPhysicsCorrections(
                telemetry,
                basicPitch,
                altitudeDifference
            );

            return ApplyPitchConstraints(physicsAdjustedPitch, altitudeDifference);
        }

        private double CalculateRequiredVerticalVelocity(
            double altitudeDifference,
            double remainingDistance,
            double horizontalSpeedMps
        )
        {
            if (
                horizontalSpeedMps < SimulationConstants.FlightPath.MIN_SPEED_MPS
                || remainingDistance < 1.0
            )
                return 0.0;

            double timeToDestination = remainingDistance / horizontalSpeedMps;
            double requiredVerticalVelocity = altitudeDifference / timeToDestination;

            double approachDistance = remainingDistance * SimulationConstants.Mathematical.REALISTIC_STOP_PRECENT;
            double smoothingFactor = Math.Min(1.0, remainingDistance / approachDistance);
            requiredVerticalVelocity *= smoothingFactor;

            return Math.Clamp(
                requiredVerticalVelocity,
                -SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS,
                SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS
            );
        }

        private double ApplyPhysicsCorrections(
            Dictionary<TelemetryFields, double> telemetry,
            double basicPitch,
            double altitudeDifference
        )
        {
            double mass = telemetry.GetValueOrDefault(TelemetryFields.Mass, 1.0);
            double weight = mass * SimulationConstants.FlightPath.GRAVITY_MPS2;

            double lift = FlightPhysicsCalculator.CalculateLift(telemetry);
            double thrust = FlightPhysicsCalculator.CalculateThrust(telemetry);
            double drag = FlightPhysicsCalculator.CalculateDrag(telemetry);

            double thrustToWeight = thrust / weight;

            if (altitudeDifference > SimulationConstants.FlightPath.ALTITUDE_PRECISION_M)
            {
                double requiredThrustRatio = Math.Sin(
                    UnitConversionHelper.ToRadians(Math.Abs(basicPitch))
                );

                if (thrustToWeight < requiredThrustRatio + 0.1)
                {
                    double fade = Math.Clamp(thrustToWeight / (requiredThrustRatio + 0.1), 0.0, 1.0);
                    basicPitch *= fade;

                    if (Math.Abs(basicPitch) < SimulationConstants.FlightPath.MIN_PITCH)
                        basicPitch = Math.Sign(basicPitch) * SimulationConstants.FlightPath.MIN_PITCH;
                }
            }
            else if (altitudeDifference < -SimulationConstants.FlightPath.ALTITUDE_PRECISION_M)
            {
                double excessThrust = thrust - drag;
                if (excessThrust > 0)
                {
                    double energyFactor = Math.Min(1.2, 1.0 + (excessThrust / weight));
                    basicPitch *= energyFactor;
                }
            }

            double liftToWeight = lift / weight;
            if (liftToWeight < 0.8)
            {
                basicPitch *= Math.Max(0.5, liftToWeight);
            }

            return basicPitch;
        }

        private double ApplyPitchConstraints(double pitch, double altitudeDifference)
        {
            double maxClimbPitch = SimulationConstants.FlightPath.MAX_CLIMB_DEG;
            double maxDescentPitch = SimulationConstants.FlightPath.MAX_DESCENT_DEG;

            if (
                Math.Abs(altitudeDifference)
                < SimulationConstants.FlightPath.ALTITUDE_PRECISION_M * 2
            )
            {
                maxClimbPitch *= 0.5;
                maxDescentPitch *= 0.5;
            }

            return Math.Clamp(pitch, -maxDescentPitch, maxClimbPitch);
        }

        private double CalculatePhysicsBasedRoll(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec,
            double newYaw,
            double pitch
        )
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

            if (speedMps <= SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double roll = 0.0;

            if (!double.IsNaN(_lastYaw))
            {
                double deltaYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, newYaw);
                double yawRate = UnitConversionHelper.ToRadians(deltaYaw) / deltaSec;

                if (Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE)
                {
                    double latAcc = speedMps * yawRate;
                    roll = UnitConversionHelper.ToDegrees(
                        Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2)
                    );
                    roll = Math.Clamp(
                        roll,
                        -SimulationConstants.FlightPath.MAX_ROLL_DEG,
                        SimulationConstants.FlightPath.MAX_ROLL_DEG
                    );
                }
                else if (
                    Math.Abs(deltaYaw) > SimulationConstants.FlightPath.MIN_RELEVENT_YAW
                    && Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE
                )
                {
                    double turnRadius = speedMps / Math.Abs(yawRate);
                    double bankAngle = UnitConversionHelper.ToDegrees(
                        Math.Atan2(
                            speedMps * speedMps,
                            turnRadius * SimulationConstants.FlightPath.GRAVITY_MPS2
                        )
                    );
                    roll =
                        Math.Sign(deltaYaw)
                        * Math.Min(bankAngle, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                }

                roll = CalculateCurveRoll(current, destination, newYaw, roll);
            }

            return roll;
        }

        private double CalculateNewYaw(double currentYaw, double targetYaw, double deltaSec)
        {
            double yawDifference = FlightPathMathHelper.CalculateAngleDifference(
                currentYaw,
                targetYaw
            );
            double turnProgress =
                Math.Abs(yawDifference)
                / SimulationConstants.FlightPath.TURN_PROGRESS_NORMALIZATION_DEG;
            double turnRateMultiplier = CalculateTurnRateMultiplier(turnProgress);
            double effectiveTurnRate =
                SimulationConstants.FlightPath.MAX_TURN_RATE_DEG_PER_SEC * turnRateMultiplier;
            double maxTurnDelta = effectiveTurnRate * deltaSec;

            if (Math.Abs(yawDifference) <= maxTurnDelta)
            {
                return targetYaw;
            }

            return currentYaw + Math.Sign(yawDifference) * maxTurnDelta;
        }

        private double CalculateTurnRateMultiplier(double turnProgress)
        {
            if (turnProgress < SimulationConstants.FlightPath.TURN_START_PHASE_THRESHOLD)
            {
                return turnProgress / SimulationConstants.FlightPath.TURN_START_PHASE_THRESHOLD;
            }

            if (turnProgress > SimulationConstants.FlightPath.TURN_END_PHASE_THRESHOLD)
            {
                return (SimulationConstants.FlightPath.FULL_TURN - turnProgress)
                    / (
                        SimulationConstants.FlightPath.FULL_TURN
                        - SimulationConstants.FlightPath.TURN_END_PHASE_THRESHOLD
                    );
            }

            return SimulationConstants.FlightPath.FULL_TURN;
        }

        private double CalculateCurveRoll(
            Location current,
            Location destination,
            double newYaw,
            double currentRoll
        )
        {
            double bearingToDestination = FlightPathMathHelper.CalculateBearing(
                current,
                destination
            );
            double yawToBearingDiff = FlightPathMathHelper.CalculateAngleDifference(
                newYaw,
                bearingToDestination
            );

            if (
                Math.Abs(yawToBearingDiff) > SimulationConstants.FlightPath.CURVE_ROLL_THRESHOLD_DEG
                && Math.Abs(currentRoll) < SimulationConstants.FlightPath.MIN_ROLL_FOR_CURVE_DEG
            )
            {
                double curveRoll =
                    Math.Sign(yawToBearingDiff)
                    * Math.Min(
                        Math.Abs(yawToBearingDiff)
                            * SimulationConstants.FlightPath.CURVE_ROLL_MULTIPLIER,
                        SimulationConstants.FlightPath.MAX_CURVE_ROLL_DEG
                    );
                return curveRoll;
            }

            return currentRoll;
        }
    }
}
