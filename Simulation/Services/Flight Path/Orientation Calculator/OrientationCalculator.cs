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
            if (remainingDistance <= 0.0)
                return 0.0;
            
            double pitch;
            
            if (altitudeDifference > 0)
            {
                double currentSpeedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
                double horizontalSpeedMps = currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
                
                if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS && remainingDistance > 0.0)
                {
                    double timeToReachTarget = remainingDistance / horizontalSpeedMps;
                    double requiredVerticalSpeedMps = altitudeDifference / timeToReachTarget;
                    
                    double requiredPitchRad = Math.Atan2(requiredVerticalSpeedMps, horizontalSpeedMps);
                    pitch = UnitConversionHelper.ToDegrees(requiredPitchRad);
                    
                    pitch = Math.Clamp(pitch, 0.0, SimulationConstants.FlightPath.MAX_CLIMB_DEG);
                }
                else
                {
                    pitch = SimulationConstants.FlightPath.MAX_CLIMB_DEG;
                }
            }
            else if (altitudeDifference < 0)
            {
                double currentSpeedKmph = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
                double horizontalSpeedMps = currentSpeedKmph / SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
                
                if (horizontalSpeedMps > SimulationConstants.FlightPath.MIN_SPEED_MPS && remainingDistance > 0.0)
                {
                    double timeToReachTarget = remainingDistance / horizontalSpeedMps;
                    double requiredVerticalSpeedMps = altitudeDifference / timeToReachTarget;
                    
                    double requiredPitchRad = Math.Atan2(requiredVerticalSpeedMps, horizontalSpeedMps);
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

        private double ApplyPhysicsCorrections(
            Dictionary<TelemetryFields, double> telemetry,
            double basicPitch,
            double altitudeDifference
        )
        {
            double mass = telemetry.GetValueOrDefault(TelemetryFields.Mass, 1.0);
            double weight = mass * SimulationConstants.FlightPath.GRAVITY_MPS2;
            double thrust = FlightPhysicsCalculator.CalculateThrust(telemetry);
            double drag = FlightPhysicsCalculator.CalculateDrag(telemetry);

            if (altitudeDifference > 0)
            {
                double maxClimbPitch = SimulationConstants.FlightPath.MAX_CLIMB_DEG;
                double pitchRad = UnitConversionHelper.ToRadians(maxClimbPitch);
                double requiredThrust = Math.Sin(pitchRad) * weight;
                
                if (thrust >= requiredThrust)
                {
                    return Math.Min(basicPitch, maxClimbPitch);
                }
                else
                {
                    double maxPossiblePitch = UnitConversionHelper.ToDegrees(Math.Asin(thrust / weight));
                    return Math.Min(basicPitch, maxPossiblePitch);
                }
            }
            else if (altitudeDifference < 0)
            {
                return Math.Max(basicPitch, -SimulationConstants.FlightPath.MAX_DESCENT_DEG);
            }

            return basicPitch;
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

            double roll = 0.0;
            if (!double.IsNaN(_lastYaw))
            {
                double dYaw = FlightPathMathHelper.CalculateAngleDifference(_lastYaw, newYaw);
                double yawRate = UnitConversionHelper.ToRadians(dYaw) / deltaSec;
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
                double angle = Math.Min(
                    Math.Abs(diff) * SimulationConstants.FlightPath.CURVE_ROLL_MULTIPLIER,
                    SimulationConstants.FlightPath.MAX_CURVE_ROLL_DEG
                );
                return Math.Sign(diff) * angle;
            }
            return currentRoll;
        }
    }
}
