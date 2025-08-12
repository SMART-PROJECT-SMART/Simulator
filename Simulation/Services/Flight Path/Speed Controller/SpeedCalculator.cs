using Shared.Common.Enums;
using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public class SpeedCalculator : ISpeedController
    {
        public double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double maxAccelerationMps2,
            double maxCruiseSpeedKmph,
            double remainingKm,
            double deltaSeconds,
            double thrustMax,
            double frontalSurface,
            double mass,
            double maxCruiseSpeed
        )
        {
            double currentSpeed = telemetry.GetValueOrDefault(
                TelemetryFields.CurrentSpeedKmph,
                0.0
            );

            double physicsAcceleration = FlightPhysicsCalculator.CalculateAcceleration(
                telemetry,
                thrustMax,
                frontalSurface,
                mass,
                maxAccelerationMps2,
                maxCruiseSpeed
            );

            double acceleration = Math.Min(physicsAcceleration, maxAccelerationMps2);

            double speedProgress = currentSpeed / maxCruiseSpeedKmph;
            double accelerationMultiplier = CalculateAccelerationMultiplier(speedProgress);
            acceleration *= accelerationMultiplier;

            double deltaSpeedKmh = (acceleration * deltaSeconds).ToMpsFromKmh();
            double newSpeed = currentSpeed + deltaSpeedKmh;

            newSpeed = Math.Clamp(
                newSpeed,
                SimulationConstants.FlightPath.MIN_SPEED_KMH,
                maxCruiseSpeedKmph
            );

            telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

            return newSpeed;
        }

        private double CalculateAccelerationMultiplier(double speedProgress)
        {
            if (speedProgress > SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD)
            {
                double decelerationRange =
                    SimulationConstants.FlightPath.FULL_DECELERATION_MULTIPLIER
                    - SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD;
                double decelerationProgress =
                    (speedProgress - SimulationConstants.FlightPath.SPEED_PROGRESS_HIGH_THRESHOLD)
                    / decelerationRange;
                return SimulationConstants.FlightPath.FULL_DECELERATION_MULTIPLIER
                    - (
                        decelerationProgress
                        * SimulationConstants.FlightPath.HIGH_SPEED_DECELERATION_FACTOR
                    );
            }

            if (speedProgress < SimulationConstants.FlightPath.SPEED_PROGRESS_LOW_THRESHOLD)
            {
                double accelerationProgress =
                    speedProgress / SimulationConstants.FlightPath.SPEED_PROGRESS_LOW_THRESHOLD;
                return SimulationConstants.FlightPath.LOW_SPEED_ACCELERATION_FACTOR
                    + (
                        accelerationProgress
                        * SimulationConstants.FlightPath.LOW_SPEED_ACCELERATION_RANGE
                    );
            }

            return SimulationConstants.FlightPath.NORMAL_ACCELERATION_MULTIPLIER;
        }
    }
}
