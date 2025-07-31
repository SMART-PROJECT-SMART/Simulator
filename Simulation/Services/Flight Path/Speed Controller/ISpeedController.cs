using Simulation.Common.Enums;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public interface ISpeedController
    {
        double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double maxAccelerationMps2,
            double maxCruiseSpeedKmph,
            double remainingKm,
            double deltaSeconds,
            double thrustMax,
            double frontalSurface,
            double mass,
            double maxCruiseSpeed
        );
    }
}
