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

            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double roll = CalculatePhysicsBasedRoll(telemetry, previous, current, destination, deltaSec, bearing, pitch);

            _lastYaw = bearing;

            return new AxisDegrees(bearing, pitch, roll);
        }

        private double CalculatePhysicsBasedRoll(
            Dictionary<TelemetryFields, double> telemetry,
            Location previous,
            Location current,
            Location destination,
            double deltaSec,
            double bearing,
            double pitch)
        {
            double speedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double speedMps = speedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;

            if (speedMps <= SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double roll = 0.0;

            if (!double.IsNaN(_lastYaw))
            {
                double deltaYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, bearing);
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
            }

            if (Math.Abs(roll) < 0.1)
            {
                roll = CalculateStraightFlightRoll(telemetry, pitch, speedMps);
            }

            return roll;
        }

        private double CalculateStraightFlightRoll(Dictionary<TelemetryFields, double> telemetry, double pitch, double speedMps)
        {
            if (Math.Abs(pitch) > 5.0)
            {
                double lift = FlightPhysicsCalculator.CalculateLift(telemetry);
                double mass = telemetry.GetValueOrDefault(TelemetryFields.Mass, 1.0);
                double weight = mass * SimulationConstants.FlightPath.GRAVITY_MPS2;
                
                double liftDeficit = weight - lift * Math.Cos(UnitConversionHelper.ToRadians(pitch));
                double rollAngle = UnitConversionHelper.ToDegrees(Math.Atan2(liftDeficit, lift));
                
                double maxRollForPitch = Math.Min(Math.Abs(pitch) * 0.5, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                return Math.Clamp(rollAngle, -maxRollForPitch, maxRollForPitch);
            }

            return 0.0;
        }
    }
}