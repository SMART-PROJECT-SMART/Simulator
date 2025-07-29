using Simulation.Common.constants;
using Simulation.Common.Enums;

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
            double maxAccel = telemetry.GetValueOrDefault(TelemetryFields.MaxAccelerationMps2, 0.0);
            double maxDecel = telemetry.GetValueOrDefault(TelemetryFields.MaxDecelerationMps2, 0.0);
            double cruiseSpeed = telemetry.GetValueOrDefault(TelemetryFields.MaxCruiseSpeedKmph, 0.0);

            double targetSpeed = cruiseSpeed;
            double diff = targetSpeed - currentSpeed;

            double accelPerSec = maxAccel * SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            double decelPerSec = Math.Abs(maxDecel) * SimulationConstants.Mathematical.FROM_KMH_TO_MPS;
            double maxDelta = diff > 0 ? accelPerSec * deltaSeconds
                                           : decelPerSec * deltaSeconds;

            double actualDelta = Math.Sign(diff) * Math.Min(Math.Abs(diff), Math.Abs(maxDelta));

            double newSpeed = currentSpeed + actualDelta;
            telemetry[TelemetryFields.CurrentSpeedKmph] = Math.Max(1.0, newSpeed);

            if (Math.Abs(actualDelta) > 0.1)
            {
                Console.WriteLine(
                    $"Speed: {currentSpeed:F1}→{newSpeed:F1} km/h (Δ{actualDelta:F1}, Rem:{remainingKm * 1000:F0}m)");
            }

            return telemetry[TelemetryFields.CurrentSpeedKmph];
        }
    }
}