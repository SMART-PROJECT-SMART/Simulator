using Simulation.Common.constants;
using Simulation.Common.Enums;
using Simulation.Services.Flight_Path.helpers;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public class SpeedController : ISpeedController
    {
        public double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double remainingKm,
            double deltaSeconds)
        {
            double currentSpeed = telemetry.GetValueOrDefault(TelemetryFields.CurrentSpeedKmph, 0.0);

            double acceleration = FlightPhysicsCalculator.CalculateAcceleration(telemetry);

            double deltaSpeedKmh = acceleration * deltaSeconds * SimulationConstants.Mathematical.FROM_MPS_TO_KMH;
            double newSpeed = currentSpeed + deltaSpeedKmh;

            double cruiseSpeed = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 0.0);
            newSpeed = Math.Clamp(newSpeed, 1.0, cruiseSpeed);

            telemetry[TelemetryFields.CurrentSpeedKmph] = newSpeed;

            return newSpeed;
        }
    }
}