using Simulation.Common.Enums;

namespace Simulation.Services.Flight_Path.Speed_Controller
{
    public interface ISpeedController
    {
        double ComputeNextSpeed(
            Dictionary<TelemetryFields, double> telemetry,
            double remainingKm,
            double deltaSeconds
        );
    }
}
