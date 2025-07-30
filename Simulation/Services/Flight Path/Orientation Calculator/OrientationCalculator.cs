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
            double altitudeDifference = destination.Altitude - current.Altitude;
            double speedMps = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0) / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            if (speedMps < SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;
            double remainingDistance = FlightPathMathHelper.CalculateDistance(current, destination);
            if (remainingDistance <= 0.0)
                return 0.0;

            double timeToDest = remainingDistance / speedMps;
            double vReq = altitudeDifference / timeToDest;
            vReq = Math.Clamp(vReq, -SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS, SimulationConstants.FlightPath.MAX_CLIMB_RATE_MPS);

            double basicPitch = UnitConversionHelper.ToDegrees(Math.Atan2(vReq, speedMps));
            double correctedPitch = ApplyPhysicsCorrections(telemetry, basicPitch, altitudeDifference);
            return Math.Clamp(correctedPitch, -SimulationConstants.FlightPath.MAX_DESCENT_DEG, SimulationConstants.FlightPath.MAX_CLIMB_DEG);
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
            double pitchRad = UnitConversionHelper.ToRadians(Math.Abs(basicPitch));
            double requiredThrust = Math.Sin(pitchRad) * weight;

            if (altitudeDifference > 0 && thrust < requiredThrust)
            {
                double scale = Math.Clamp(thrust / requiredThrust, 0.0, 1.0);
                basicPitch *= scale;
            }
            else if (altitudeDifference < 0 && (thrust - drag) > 0)
            {
                basicPitch *= 1 + ((thrust - drag) / weight);
            }

            return basicPitch;
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
            double speedMps = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0) / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            if (speedMps <= SimulationConstants.FlightPath.MIN_SPEED_MPS)
                return 0.0;

            double roll = 0.0;
            if (!double.IsNaN(_lastYaw))
            {
                double dYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, newYaw);
                double yawRate = UnitConversionHelper.ToRadians(dYaw) / deltaSec;
                if (Math.Abs(yawRate) > SimulationConstants.FlightPath.MIN_YAW_RATE)
                {
                    double latAcc = speedMps * yawRate;
                    roll = UnitConversionHelper.ToDegrees(Math.Atan2(latAcc, SimulationConstants.FlightPath.GRAVITY_MPS2));
                    roll = Math.Clamp(roll, -SimulationConstants.FlightPath.MAX_ROLL_DEG, SimulationConstants.FlightPath.MAX_ROLL_DEG);
                }
                roll = CalculateCurveRoll(current, destination, newYaw, roll);
            }
            return roll;
        }

        private double CalculateNewYaw(double currentYaw, double targetYaw, double deltaSec)
        {
            double diff = FlightPathMathHelper.CalculateAngleDifference(currentYaw, targetYaw);
            double turnRate = SimulationConstants.FlightPath.MAX_TURN_RATE_DEG_PER_SEC;
            double maxDelta = turnRate * deltaSec;
            if (Math.Abs(diff) <= maxDelta)
                return targetYaw;
            return currentYaw + Math.Sign(diff) * maxDelta;
        }

        private double CalculateCurveRoll(Location current, Location destination, double newYaw, double currentRoll)
        {
            double bearing = FlightPathMathHelper.CalculateBearing(current, destination);
            double diff = FlightPathMathHelper.CalculateAngleDifference(newYaw, bearing);
            if (Math.Abs(diff) > SimulationConstants.FlightPath.CURVE_ROLL_THRESHOLD_DEG)
            {
                double angle = Math.Min(Math.Abs(diff) * SimulationConstants.FlightPath.CURVE_ROLL_MULTIPLIER, SimulationConstants.FlightPath.MAX_CURVE_ROLL_DEG);
                return Math.Sign(diff) * angle;
            }
            return currentRoll;
        }
    }
}
