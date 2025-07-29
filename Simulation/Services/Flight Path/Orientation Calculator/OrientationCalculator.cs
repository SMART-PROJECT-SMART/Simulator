using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;
using Simulation.Services.helpers;
using Simulation.Models;

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
            double deltaSec)
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double pitch = telemetry.GetValueOrDefault(TelemetryFields.PitchDeg, 0.0);
            double currentYaw = telemetry.GetValueOrDefault(TelemetryFields.YawDeg, 0.0);

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            
            double targetYaw = bearing;
            double yawDifference = FlightPathMathHelper.CalculateAngleDifference(currentYaw, targetYaw);
            
            double maxTurnRate = 8.0; 
            double turnProgress = Math.Abs(yawDifference) / 90.0;
            
            double turnRateMultiplier = 1.0;
            if (turnProgress < 0.3)
            {
                turnRateMultiplier = turnProgress / 0.3;
            }
            else if (turnProgress > 0.7)
            {
                turnRateMultiplier = (1.0 - turnProgress) / 0.3;
            }
            
            double effectiveTurnRate = maxTurnRate * turnRateMultiplier;
            double maxTurnDelta = effectiveTurnRate * deltaSec;
            
            double newYaw;
            if (Math.Abs(yawDifference) <= maxTurnDelta)
            {
                newYaw = targetYaw;
            }
            else
            {
                newYaw = currentYaw + Math.Sign(yawDifference) * maxTurnDelta;
            }
            
            double roll = CalculatePhysicsBasedRoll(telemetry, previous, current, destination, deltaSec, newYaw, pitch);

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
            double pitch)
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
                        Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2));
                    roll = Math.Clamp(roll, -SimulationConstants.FlightPath.MAX_ROLL_DEG, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                }
                else if (Math.Abs(deltaYaw) > 0.1 && Math.Abs(yawRate) > 0.001)
                {
                    double turnRadius = speedMps / Math.Abs(yawRate);
                    double bankAngle = UnitConversionHelper.ToDegrees(
                        Math.Atan2(speedMps * speedMps, turnRadius * SimulationConstants.FlightPath.GRAVITY_MPS2));
                    roll = Math.Sign(deltaYaw) * Math.Min(bankAngle, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                }
                
                double bearingToDestination = FlightPathMathHelper.CalculateBearing(current, destination);
                double yawToBearingDiff = FlightPathMathHelper.CalculateAngleDifference(newYaw, bearingToDestination);
                
                if (Math.Abs(yawToBearingDiff) > 1.0 && Math.Abs(roll) < 2.0)
                {
                    double curveRoll = Math.Sign(yawToBearingDiff) * Math.Min(Math.Abs(yawToBearingDiff) * 0.5, 3.0);
                    roll = curveRoll;
                }
            }

            return roll;
        }

        private double CalculateStraightFlightRoll(Dictionary<TelemetryFields, double> telemetry, double pitch, double speedMps)
        {
            return 0.0;
        }
    }
}