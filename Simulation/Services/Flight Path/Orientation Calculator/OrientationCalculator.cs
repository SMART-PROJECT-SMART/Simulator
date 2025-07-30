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
            double pitch = telemetry.GetValueOrDefault(TelemetryFields.PitchDeg, 0.0);
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
