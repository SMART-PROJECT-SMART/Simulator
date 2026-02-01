using Core.Models;
using Simulation.Models;

namespace Simulation.Services.TelemetryDeviceClient
{
    public interface ITelemetryDeviceClient
    {
        Task<bool> CreateTelemetryDeviceAsync(
            int tailId,
            IEnumerable<int> portNumbers,
            Location location,
            CancellationToken cancellationToken = default
        );
    }
}
