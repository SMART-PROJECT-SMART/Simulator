using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public class SpeedCalculator : ISpeedController
    {

        public double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double remainingKm,
            double deltaSeconds)
        {
            double currentSpeed = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);
            double maxAcceleration = telemetry.GetValueOrDefault(TelemetryFields.MaxAccelerationMps2, 2.0);
            double cruiseSpeed = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 180.0);

            double physicsAcceleration = FlightPhysicsCalculator.CalculateAcceleration(telemetry);
            double acceleration = Math.Min(physicsAcceleration, maxAcceleration);
            
            double speedProgress = currentSpeed / cruiseSpeed; 
            double accelerationMultiplier = CalculateAccelerationMultiplier(speedProgress);
            acceleration *= accelerationMultiplier;
            
            double deltaSpeedKmh = acceleration * deltaSeconds * SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
            double newSpeed = currentSpeed + deltaSpeedKmh;

            newSpeed = Math.Clamp(newSpeed, SimulationConstants.FlightPath.MIN_SPEED_KMH, cruiseSpeed);

            telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

            return newSpeed;
        }

        private double CalculateAccelerationMultiplier(double speedProgress)
        {
            if (speedProgress > SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD)
            {
                double decelerationRange = SimulationConstants.FlightPath.FULL_DECELERATION_MULTIPLIER - SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD;
                double decelerationProgress = (speedProgress - SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD) / decelerationRange;
                return SimulationConstants.FlightPath.FULL_DECELERATION_MULTIPLIER - (decelerationProgress * SimulationConstants.FlightPath.HIGH_SPEED_DECELERATION_FACTOR);
            }
            
            if (speedProgress < SimulationConstants.FlightPath.SPEED_PROGRESS_LOW_THRESHOLD)
            {
                double accelerationProgress = speedProgress / SimulationConstants.FlightPath.SPEED_PROGRESS_LOW_THRESHOLD;
                return SimulationConstants.FlightPath.LOW_SPEED_ACCELERATION_FACTOR + (accelerationProgress * SimulationConstants.FlightPath.LOW_SPEED_ACCELERATION_RANGE);
            }
            
            return SimulationConstants.FlightPath.NORMAL_ACCELERATION_MULTIPLIER;
        }
    }
}