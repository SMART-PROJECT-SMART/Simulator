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
            
            double accelerationMultiplier = 1.0;
            if (speedProgress > 0.7)
            {
                accelerationMultiplier = 1.0 - ((speedProgress - 0.7) / 0.3) * 0.5;
            }
            else if (speedProgress < 0.3)
            {
                accelerationMultiplier = 0.7 + (speedProgress / 0.3) * 0.3;
            }
            
            acceleration *= accelerationMultiplier;
            
            double deltaSpeedKmh = acceleration * deltaSeconds * SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
            double newSpeed = currentSpeed + deltaSpeedKmh;

            newSpeed = Math.Clamp(newSpeed, SimulationConstants.FlightPath.MIN_SPEED_KMH, cruiseSpeed);

            telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

            return newSpeed;
        }
    }
}